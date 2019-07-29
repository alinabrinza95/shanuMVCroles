using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using shanuMVCUserRoles.Models;
using System.Collections.Generic;
using System.Net.Mail;

namespace shanuMVCUserRoles.Controllers
{
    public class TimeSheetViewModelsController : Controller
    {
        private int _daysOffHoliday;
        private int _daysOffFullMedical;
        private int _daysOffPartialMedical;
        private int _daysOffNotPaid;
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {

            return View(db.TimeSheetViewModel.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var timeSheetViewModel = db.TimeSheetViewModel.Find(id);
            if (timeSheetViewModel == null)
            {
                return HttpNotFound();
            }
            return View(timeSheetViewModel);
        }

        public ActionResult Create()
        {
            ViewBag.Name = Pontaj();    

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Mark,FirstName,LastName,CNP,TeamLeaderEmail,Flag")] TimeSheetViewModel timeSheetViewModel)
        {
            if (ModelState.IsValid)
            {
                db.TimeSheetViewModel.Add(timeSheetViewModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(timeSheetViewModel);
        }


        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var timeSheetViewModel = db.TimeSheetViewModel.Find(id);
            if (timeSheetViewModel == null)
            {
                return HttpNotFound();
            }
            return View(timeSheetViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Mark,FirstName,LastName,CNP,TeamLeaderEmail,Flag")] TimeSheetViewModel timeSheetViewModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(timeSheetViewModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(timeSheetViewModel);
        }


        public List<HolidayViewModel> Holiday()
        {
            var holiday = from b in db.AspNetHolidays
                          join c in db.Users on b.Email equals c.Email
                          where (c.UserName.Equals(User.Identity.Name) && (b.StartDate.Month.Equals(DateTime.Now.Month) || b.EndDate.Month.Equals(DateTime.Now.Month)) 
                          && b.Flag==true && b.HolidayType!="Concediu medical" && b.HolidayType!="Concediu fara plata")
                          select b;
            return holiday.ToList();
        }
        public List<HolidayViewModel> FullMedicalHoliday()
        {
            var holiday = from b in db.AspNetHolidays
                          join c in db.Users on b.Email equals c.Email
                          where (c.UserName.Equals(User.Identity.Name) && (b.StartDate.Month.Equals(DateTime.Now.Month) || b.EndDate.Month.Equals(DateTime.Now.Month))
                          && b.Flag == true && b.HolidayType=="Concediu medical" && 
                          (b.SickLeaveIndex==2|| b.SickLeaveIndex == 3
                          || b.SickLeaveIndex == 4 || b.SickLeaveIndex == 5
                          || b.SickLeaveIndex == 6 || b.SickLeaveIndex == 12
                          || b.SickLeaveIndex == 14))
                          select b;
            return holiday.ToList();
        }
        public List<HolidayViewModel> PartialMedicalHoliday()
        {
            var holiday = from b in db.AspNetHolidays
                          join c in db.Users on b.Email equals c.Email
                          where (c.UserName.Equals(User.Identity.Name) && (b.StartDate.Month.Equals(DateTime.Now.Month) || b.EndDate.Month.Equals(DateTime.Now.Month))
                          && b.Flag == true && b.HolidayType == "Concediu medical" &&
                          (b.SickLeaveIndex == 1 || b.SickLeaveIndex == 7
                          || b.SickLeaveIndex == 13 || b.SickLeaveIndex == 15))
                          select b;
            return holiday.ToList();
        }
        public List<HolidayViewModel> NotPaidHoliday()
        {
            var holiday = from b in db.AspNetHolidays
                          join c in db.Users on b.Email equals c.Email
                          where (c.UserName.Equals(User.Identity.Name) && (b.StartDate.Month.Equals(DateTime.Now.Month) || b.EndDate.Month.Equals(DateTime.Now.Month))
                          && b.Flag == true && b.HolidayType == "Concediu fara plata")
                          select b;
            return holiday.ToList();
        }
        public List<OOHRequestViewModel> Ooh()
        {
            var ooh = from b in db.OOHRequestViewModel
                      join c in db.Users on b.Email equals c.Email
                      where (c.UserName.Equals(User.Identity.Name) && b.Day.Month.Equals(DateTime.Now.Month) && b.Flag==true)
                      select b;
            return ooh.ToList();
        }

        private static int ComputeNumberOfDaysInMonth()
        {
            return DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
        }

        private static int ComputeNumberOfBusinessDaysInMonth(int daysInMonth)
        {
            var businessDaysInMonth = 0;
            var firstDayInMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var lastDayInMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, daysInMonth);
            for (var date = firstDayInMonth; date < lastDayInMonth; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    businessDaysInMonth++;
                }
            }

            return businessDaysInMonth;
        }

        private static int ComputeNumberOfHolidayDays(bool holidayType, List<HolidayViewModel> holidayDays)
        {
            var totalNumberOfHolidayDays = 0;
            if (!holidayType)
            {
                return totalNumberOfHolidayDays;
            }
            foreach (var hol in holidayDays)
            {
                for (var date = hol.StartDate; date <= hol.EndDate; date = date.AddDays(1))
                {
                    if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                    {
                        totalNumberOfHolidayDays++;
                    }
                }
            }

            return totalNumberOfHolidayDays;
        }

        private static double ComputeOohHours(List<OOHRequestViewModel> ooh)
        {
            double bonusHours = 0;
            if (!ooh.Any())
            {
                return bonusHours;
            }
            foreach (var oohObject in ooh)
            {
                bonusHours += (oohObject.Hours * 2);
            }

            return bonusHours;
        }

        private void ComputeAllHolidayTypes(List<HolidayViewModel> holiday, List<HolidayViewModel> fullMedicalHoliday,
            List<HolidayViewModel> partialMedicalHoliday, List<HolidayViewModel> notPaidHoliday)
        {
            _daysOffHoliday = ComputeNumberOfHolidayDays(holiday.Any(), holiday);
            _daysOffFullMedical = ComputeNumberOfHolidayDays(fullMedicalHoliday.Any(), fullMedicalHoliday);
            _daysOffPartialMedical = ComputeNumberOfHolidayDays(partialMedicalHoliday.Any(), partialMedicalHoliday);
            _daysOffNotPaid = ComputeNumberOfHolidayDays(notPaidHoliday.Any(), notPaidHoliday);
        }

        private int ComputeDaysForTimeSheet()
        {
            var holiday = Holiday();
            var fullMedicalHoliday = FullMedicalHoliday();
            var partialMedicalHoliday = PartialMedicalHoliday();
            var notPaidHoliday = NotPaidHoliday();

            var daysInMonth = ComputeNumberOfDaysInMonth();
            var businessDaysInMonth = ComputeNumberOfBusinessDaysInMonth(daysInMonth);

            ComputeAllHolidayTypes(holiday, fullMedicalHoliday, partialMedicalHoliday, notPaidHoliday);

            return businessDaysInMonth;
        }

        public string PontajV2()
        {
            var bonusHours = ComputeOohHours(Ooh());
            var businessDaysInMonth = ComputeDaysForTimeSheet();

            var workingHoursThisMonth = businessDaysInMonth * 8;
            var hoursWorked = (businessDaysInMonth - _daysOffHoliday - _daysOffFullMedical - _daysOffPartialMedical - _daysOffNotPaid) * 8 + Convert.ToInt32(bonusHours);
            var hoursPaid = (hoursWorked - Convert.ToInt32(bonusHours)) + _daysOffHoliday * 8 + _daysOffFullMedical * 8 + _daysOffPartialMedical * 0.75 * 8 + bonusHours * 2;
            var transport = (float)((float)(businessDaysInMonth - _daysOffHoliday - _daysOffFullMedical - _daysOffPartialMedical - _daysOffNotPaid) / (float)businessDaysInMonth) * (float)80.0;
            var mealTickets = (businessDaysInMonth - _daysOffHoliday - _daysOffFullMedical - _daysOffPartialMedical - _daysOffNotPaid);


            return "Ore lucratoare in luna: " + workingHoursThisMonth + " Ore lucrate in luna: " + hoursWorked
                + " Ore platite in luna: " + hoursPaid + " Decont transport: " + transport + " Tichete de masa: " + mealTickets;
        }

        public string Pontaj()
        {
            bool holiday = Holiday().Count > 0;            
            bool ooh = Ooh().Count > 0;

            int daysInMonth = System.DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);                       
            var businessDaysInMonth = 0;     
            for (var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1); date < new DateTime(DateTime.Now.Year, DateTime.Now.Month, daysInMonth); date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday
                    && date.DayOfWeek != DayOfWeek.Sunday)
                    businessDaysInMonth++;
            }
            int mealTickets = businessDaysInMonth;
            int workingHoursInMonth = businessDaysInMonth * 8;
            int daysOff = 0;            
            double bonusHours = 0;
            double hoursWorked = 0;
            double hoursPaid = 0;
            double transport = 80.0;


            //if the employee had days off and extra hours worked
            if (holiday && ooh == true)
            {
                //compute days off
                foreach (var hol in Holiday())
                {
                    for (var date = hol.StartDate; date <= hol.EndDate; date = date.AddDays(1))
                    {
                     if (date.DayOfWeek != DayOfWeek.Saturday
                     && date.DayOfWeek != DayOfWeek.Sunday) daysOff++;
                    }
                }
                //compute extra hours worked
                foreach (var objectooh in Ooh())
                {
                 bonusHours += (objectooh.Hours * 2);
                }

                hoursWorked = (businessDaysInMonth - daysOff) * 8 + bonusHours;
                hoursPaid = businessDaysInMonth * 8 + bonusHours * 2;
                transport = (float)((float)(businessDaysInMonth - daysOff) / (float)businessDaysInMonth) * (float)transport;
                mealTickets = mealTickets - daysOff;
            }

            //if the employee had days off but no extra hours worked
            if (holiday == true && ooh==false)
            {
                //compute days off
                foreach (var hol in Holiday())
                {
                for (var date = hol.StartDate; date <= hol.EndDate; date = date.AddDays(1))
                {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday) daysOff++;
                 }
                }
                hoursWorked = (businessDaysInMonth - daysOff) * 8;
                hoursPaid = businessDaysInMonth * 8;
                transport = (float)((float)(businessDaysInMonth - daysOff) / (float)businessDaysInMonth) * (float)transport;
                mealTickets = mealTickets - daysOff;
            }

            //if the employee had no days off but extra hours worked
            if (holiday ==false && ooh == true)
            {
               
                //compute extra hours worked
                foreach (var objectooh in Ooh())
                {
                bonusHours += (objectooh.Hours * 2);
                }

             hoursWorked = businessDaysInMonth * 8 + bonusHours/2;
             hoursPaid = businessDaysInMonth * 8 + bonusHours;                
            }

            //if the employee had no days off and no extra hours worked
            if (holiday&&ooh == false)
            {
             hoursWorked = workingHoursInMonth;
             hoursPaid = workingHoursInMonth;
            }

            return "Ore lucratoare in luna: " + workingHoursInMonth + " Ore lucrate in luna: " + hoursWorked
                + " Ore platite in luna: " + hoursPaid + " Transport: " + transport + " Bonuri de masa: " + mealTickets;
        }

        public ActionResult SendMail()
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

            mail.From = new MailAddress("internalapppontaj@gmail.com");
            mail.To.Add("myheroalinabrinza95@gmail.com");
            mail.Subject = "Test Mail";
            mail.Body = PontajV2();

            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("internalapppontaj@gmail.com", "SCCpontaj2017$");
            SmtpServer.EnableSsl = true;


            SmtpServer.Send(mail);
            return RedirectToAction("SuccessPontaj", "Success");
        }


        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimeSheetViewModel timeSheetViewModel = db.TimeSheetViewModel.Find(id);
            if (timeSheetViewModel == null)
            {
                return HttpNotFound();
            }
            return View(timeSheetViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TimeSheetViewModel timeSheetViewModel = db.TimeSheetViewModel.Find(id);
            db.TimeSheetViewModel.Remove(timeSheetViewModel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

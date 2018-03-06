{
	"Name": "Example Holiday", //Name of the holiday
	"Comment": "First day of year",
	"Open": null, //Open time   (null = N/A), in case the markets are partially opened
	"Closed": null, //Closed time (null = N/A), in case the markets are partially opened
	"Date": {
		"Day": 0, //Day of the month
		"DayOccurance": 0, //Occurance of day number (1 == 1st monday, 2 == 2nd monday of the month)
		"IsLastDayOccurance": false, //Last x day of the month
		"DayOfWeek": "Monday", //Which day it will always be
		"AfterHoliday": "Easter",
		"BeforeHoliday": "Easter",
		"DaysAfterHoliday": 1, //Happens x days after another holiday, should be present int this list
		"DaysBeforeHoliday": 1, //Happens x days before another holiday, should be present int this list
		"EveryXYear": 0, //Happens each x year
		"LastFullWeekOfMonth": false, //Should happen in the last week of the month
		"Month": 2, //In month
		"StartYear": 0, //Start of year we started counting x of each year
		"WeekOfMonth": 0 //Which week of the month
	}
}

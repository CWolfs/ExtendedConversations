using System;

namespace ExtendedConversations.Core {
  public static class DateHelper {
    // Helper method to parse flexible date formats (yyyy, yyyy-MM, yyyy-M, yyyy-MM-dd, etc.)
    public static bool TryParseFlexibleDate(string dateString, out DateTime parsedDate) {
      parsedDate = DateTime.MinValue;

      if (string.IsNullOrEmpty(dateString)) {
        return false;
      }

      switch (dateString.Length) {
        case 4: // year only
          if (int.TryParse(dateString, out int yearOnly)) {
            parsedDate = new DateTime(yearOnly, 1, 1).Date;
            return true;
          }
          return false;

        case 6: // year and month (yyyy-M format like "3050-6")
        case 7: // year and month (yyyy-MM format like "3050-06")
          string[] yearMonthFormats = { "yyyy-MM", "yyyy-M" };
          if (DateTime.TryParseExact(dateString, yearMonthFormats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsedDate)) {
            parsedDate = parsedDate.Date; // Strip time component
            return true;
          }
          return false;

        case 8: // year, month and day (yyyy-M-d format like "3050-6-5")
        case 9: // year, month and day (yyyy-MM-d or yyyy-M-dd format)
        case 10: // year, month and day (yyyy-MM-dd format like "3050-06-05")
          string[] fullDateFormats = { "yyyy-MM-dd", "yyyy-M-d", "yyyy-MM-d", "yyyy-M-dd" };
          if (DateTime.TryParseExact(dateString, fullDateFormats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsedDate)) {
            parsedDate = parsedDate.Date; // Strip time component
            return true;
          }
          return false;

        default:
          return false;
      }
    }

    // Helper enum to determine the precision of a date string
    public enum DatePrecision {
      Year,      // yyyy
      Month,     // yyyy-MM or yyyy-M
      Day        // yyyy-MM-dd, yyyy-M-d, etc.
    }

    // Helper method to determine the precision of a date string
    public static DatePrecision GetDatePrecision(string dateString) {
      if (string.IsNullOrEmpty(dateString)) {
        return DatePrecision.Day; // default
      }

      if (dateString.Length == 4 && !dateString.Contains("-")) {
        return DatePrecision.Year;
      }

      int dashCount = dateString.Split('-').Length - 1;
      if (dashCount == 1) {
        return DatePrecision.Month;
      }

      return DatePrecision.Day;
    }

    // Helper method to compare dates at a specific precision level (fuzzy comparison)
    public static bool FuzzyDateEquals(DateTime date1, DateTime date2, DatePrecision precision) {
      switch (precision) {
        case DatePrecision.Year:
          return date1.Year == date2.Year;
        case DatePrecision.Month:
          return date1.Year == date2.Year && date1.Month == date2.Month;
        case DatePrecision.Day:
          return date1.Date == date2.Date;
        default:
          return date1.Date == date2.Date;
      }
    }

    // Helper method to try fuzzy date comparison (compares at the precision of the less precise date)
    public static bool TryFuzzyDateComparison(string value1, string value2, out bool areEqual) {
      areEqual = false;

      // Try to parse both values as dates
      if (!TryParseFlexibleDate(value1, out DateTime date1)) {
        return false;
      }
      if (!TryParseFlexibleDate(value2, out DateTime date2)) {
        return false;
      }

      // Determine precision of both dates and use the less precise one
      DatePrecision precision1 = GetDatePrecision(value1);
      DatePrecision precision2 = GetDatePrecision(value2);
      DatePrecision comparePrecision = (DatePrecision)Math.Min((int)precision1, (int)precision2);

      areEqual = FuzzyDateEquals(date1, date2, comparePrecision);
      return true;
    }

    // Helper method for fuzzy date comparison with ordering (returns -1, 0, or 1)
    // Compares at the precision of the less precise date
    // Returns: -1 if date1 < date2, 0 if equal, 1 if date1 > date2
    public static bool TryFuzzyDateCompare(string value1, string value2, out int compareResult) {
      compareResult = 0;

      // Try to parse both values as dates
      if (!TryParseFlexibleDate(value1, out DateTime date1)) {
        return false;
      }
      if (!TryParseFlexibleDate(value2, out DateTime date2)) {
        return false;
      }

      // Determine precision of both dates and use the less precise one
      DatePrecision precision1 = GetDatePrecision(value1);
      DatePrecision precision2 = GetDatePrecision(value2);
      DatePrecision comparePrecision = (DatePrecision)Math.Min((int)precision1, (int)precision2);

      // Extract the relevant components based on precision
      DateTime normalizedDate1;
      DateTime normalizedDate2;

      switch (comparePrecision) {
        case DatePrecision.Year:
          normalizedDate1 = new DateTime(date1.Year, 1, 1);
          normalizedDate2 = new DateTime(date2.Year, 1, 1);
          break;
        case DatePrecision.Month:
          normalizedDate1 = new DateTime(date1.Year, date1.Month, 1);
          normalizedDate2 = new DateTime(date2.Year, date2.Month, 1);
          break;
        case DatePrecision.Day:
        default:
          normalizedDate1 = date1.Date;
          normalizedDate2 = date2.Date;
          break;
      }

      compareResult = normalizedDate1.CompareTo(normalizedDate2);
      return true;
    }
  }
}

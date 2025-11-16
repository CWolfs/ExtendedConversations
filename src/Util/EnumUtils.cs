using System;
using System.Linq;

public static class EnumUtils {
  public static bool IsDefined<T>(int value) {
    var values = Enum.GetValues(typeof(T)).Cast<int>().OrderBy(x => x);

    return values.Contains(value);
  }
}
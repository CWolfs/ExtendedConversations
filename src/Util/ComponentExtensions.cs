using UnityEngine;

public static class ComponentExtensions {
  public static T CopyComponent<T>(this GameObject destination, T original) where T : Component {
    System.Type type = original.GetType();
    var dst = destination.GetComponent(type) as T;
    if (!dst) dst = destination.AddComponent(type) as T;
    var fields = type.GetFields();
    foreach (var field in fields) {
      if (field.IsStatic) continue;
      field.SetValue(dst, field.GetValue(original));
    }
    var props = type.GetProperties();
    foreach (var prop in props) {
      if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name") continue;
      prop.SetValue(dst, prop.GetValue(original, null), null);
    }
    return dst as T;
  }
}
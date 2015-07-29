using System;

namespace csgl {
  public class ValueFloat {
    public float value = 0.0f;

    public ValueFloat() {
    }

    public static float operator*( ValueFloat a, float b ) {
      return a.value * b;
    }

    public static explicit operator float( ValueFloat a ) {
      return a.value;
    }

    public static explicit operator double( ValueFloat a ) {
      return a.value;
    }
  }
}


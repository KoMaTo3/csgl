using System;
using OpenTK;

namespace csgl {
  public class ValueMatrix4 {
    public Matrix4 value = Matrix4.Identity;

    public ValueMatrix4() {
    }


    public static explicit operator Matrix4( ValueMatrix4 a ) {
      return a.value;
    }
  }
}


using System;

namespace csgl {
  public struct Vector4UInt32 {
    public UInt32 x;
    public UInt32 y;
    public UInt32 z;
    public UInt32 w;

    public Vector4UInt32( UInt32 setX = 0, UInt32 setY = 0, UInt32 setZ = 0, UInt32 setW = 0 ) {
      this.x = setX;
      this.y = setY;
      this.z = setZ;
      this.w = setW;
    }
  }
}


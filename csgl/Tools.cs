using System;
using OpenTK;

namespace csgl {
  public class Tools {
    static public Matrix4 MakeMatrix( Vector3 position, Vector3 scale, float rotation ) {
      Matrix4 matrixTranslation = new Matrix4(
                                    1.0f, 0, 0, position.X,
                                    0, 1.0f, 0, position.Y,
                                    0, 0, 1.0f, 0,
                                    0, 0, 0, 1.0f
                                  );
      Matrix4 matrixScale = new Matrix4(
                              scale.X, 0, 0, 0,
                              0, scale.Y, 0, 0,
                              0, 0, scale.Z, 0,
                              0, 0, 0, 1.0f
                            );
      float sinA = ( float ) Math.Sin( rotation );
      float cosA = ( float ) Math.Cos( rotation );
      Matrix4 matrixRotation = new Matrix4(
                                 cosA, -sinA, 0, 0,
                                 sinA, cosA, 0, 0,
                                 0, 0, 1.0f, 0,
                                 0, 0, 0, 1.0f );
      return matrixTranslation * matrixRotation * matrixScale;
    }

    private Tools() {
    }
  }
}


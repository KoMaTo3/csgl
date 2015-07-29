using System;
using OpenTK;

namespace csgl {
  public class Tools {
    const float deg2rad = ( float ) Math.PI / 180.0f;

    static public Matrix4 MakeMatrix( Vector3 position, Vector3 scale, float rotation ) {
      Matrix4 matrixTranslation = new Matrix4(
                                    1.0f, 0, 0, position.X,
                                    0, 1.0f, 0, position.Y,
                                    0, 0, 1.0f, position.Z,
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

    static public Matrix4 MakeMatrix( Vector3 position, Vector3 scale, Vector3 rotationQuaternion ) {
      Matrix4 matrixTranslation = new Matrix4(
                                    1.0f, 0, 0, position.X,
                                    0, 1.0f, 0, position.Y,
                                    0, 0, 1.0f, position.Z,
                                    0, 0, 0, 1.0f
                                  );
      Matrix4 matrixScale = new Matrix4(
                              scale.X, 0, 0, 0,
                              0, scale.Y, 0, 0,
                              0, 0, scale.Z, 0,
                              0, 0, 0, 1.0f
                            );

      float sx, cx, sy, cy, sz, cz;
      float sxcy, cxcy, sxsy, cxsy;

      sx = ( float ) Math.Sin( rotationQuaternion.X * 0.5f );
      cx = ( float ) Math.Cos( rotationQuaternion.X * 0.5f );
      sy = ( float ) Math.Sin( rotationQuaternion.Y * 0.5f );
      cy = ( float ) Math.Cos( rotationQuaternion.Y * 0.5f );
      sz = ( float ) Math.Sin( rotationQuaternion.Z * 0.5f );
      cz = ( float ) Math.Cos( rotationQuaternion.Z * 0.5f );
      //Math::SinCos( DEG2RAD( yaw ) * 0.5f, sz, cz );
      //Math::SinCos( DEG2RAD( pitch ) * 0.5f, sy, cy );
      //Math::SinCos( DEG2RAD( roll ) * 0.5f, sx, cx );

      sxcy = sx * cy;
      cxcy = cx * cy;
      sxsy = sx * sy;
      cxsy = cx * sy;

      Quaternion rotationQuat = Quaternion.Identity;
      rotationQuat.X = cxsy * sz - sxcy * cz;
      rotationQuat.Y = -cxsy * cz - sxcy * sz;
      rotationQuat.Z = sxsy * cz - cxcy * sz;
      rotationQuat.W = cxcy * cz + sxsy * sz;

      //Mat4 matRotFromQuat( rotation.ToMat4() );
      //Matrix4 matRotFromQuat = Matrix4.Identity;
      Matrix4 matRotFromQuat = Matrix4.CreateFromQuaternion( rotationQuat );

      /* float sinA = ( float ) Math.Sin( rotation );
      float cosA = ( float ) Math.Cos( rotation );
      Matrix4 matrixRotation = new Matrix4(
        cosA, -sinA, 0, 0,
        sinA, cosA, 0, 0,
        0, 0, 1.0f, 0,
        0, 0, 0, 1.0f ); */
      return matrixTranslation * matRotFromQuat * matrixScale;
    }

    static public Matrix4 MakeMatrixProjectionPerspective( float planeNear = 0.01f, float planeFar = 1000.0f, float FOV = ( float ) Math.PI * 0.5f, float aspect = 1.0f ) {
      Matrix4 matrixProjection = Matrix4.Identity;
      float h = 1.0f / ( float ) Math.Tan( FOV * 0.5f );
      float negDepth = planeNear - planeFar;

      matrixProjection.Row0[ 0 ] = h / aspect;
      matrixProjection.Row1[ 1 ] = h;
      matrixProjection.Row2[ 2 ] = ( planeFar + planeNear ) / negDepth;
      matrixProjection.Row2[ 3 ] = -1.0f;
      matrixProjection.Row3[ 2 ] = 2.0f * ( planeNear * planeFar ) / negDepth;
      matrixProjection.Row3[ 3 ] = 0.0f;
      //matrixProjection.Transpose();
      return matrixProjection;
    }

    static public Matrix4 MakeMatrixRotation( Vector3 quaternionRotation ) {
      float sx, cx, sy, cy, sz, cz;
      float sxcy, cxcy, sxsy, cxsy;

      sx = ( float ) Math.Sin( quaternionRotation.X * 0.5f );
      cx = ( float ) Math.Cos( quaternionRotation.X * 0.5f );
      sy = ( float ) Math.Sin( quaternionRotation.Y * 0.5f );
      cy = ( float ) Math.Cos( quaternionRotation.Y * 0.5f );
      sz = ( float ) Math.Sin( quaternionRotation.Z * 0.5f );
      cz = ( float ) Math.Cos( quaternionRotation.Z * 0.5f );

      sxcy = sx * cy;
      cxcy = cx * cy;
      sxsy = sx * sy;
      cxsy = cx * sy;

      Quaternion rotationQuat = Quaternion.Identity;
      rotationQuat.X = cxsy * sz - sxcy * cz;
      rotationQuat.Y = -cxsy * cz - sxcy * sz;
      rotationQuat.Z = sxsy * cz - cxcy * sz;
      rotationQuat.W = cxcy * cz + sxsy * sz;

      return Matrix4.CreateFromQuaternion( rotationQuat );
    }

    static public Matrix4 MakeMatrixPerspective( Vector3 position, Vector3 rotation, float planeNear = 0.01f, float planeFar = 1000.0f, float FOV = ( float ) Math.PI * 0.5f, float aspect = 1.0f ) {
      Matrix4 matTranslation = Matrix4.Identity;
      matTranslation.Row3[ 0 ] = position.X;
      matTranslation.Row3[ 1 ] = position.Y;
      matTranslation.Row3[ 2 ] = position.Z;

      Matrix4 matScale = Matrix4.Identity;

      Matrix4 matrixProjection = Tools.MakeMatrixProjectionPerspective( planeNear, planeFar, FOV, aspect );
      matrixProjection.Transpose();
      Matrix4 matrixWorld = Matrix4.Identity;

      //Mat4 matRotFromQuat( rotation.ToMat4() );
      //Matrix4 matRotFromQuat = Matrix4.Identity;
      Matrix4 matRotFromQuat = Tools.MakeMatrixRotation( rotation );
      //matRotFromQuat.Transpose();

      //matrixWorld = matRotFromQuat * matTranslation;
      matrixWorld = matScale * matRotFromQuat * matTranslation * matrixProjection;
      matrixWorld.Transpose();

      return matrixWorld;
    }

    static public float Deg2Rag( float degree ) {
      return ( float ) degree * Tools.deg2rad;
    }

    private Tools() {
    }
  }
}


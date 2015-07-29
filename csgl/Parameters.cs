using System;
using OpenTK;

namespace csgl {
  public class Parameters {
    public static ValueMatrix4 worldMatrix;
    public static ValueMatrix4 projectionMatrix;
    public static TextureBufferObject tboObjectsMatrices;

    private Parameters() {
    }

    static public UInt32 GetRenderableObjectIndex() {
      return Parameters.tboObjectsMatrices.GetFreeIndex();
    }

    static public void FreeRenderableObjectIndex( UInt32 index ) {
      Console.WriteLine( "[NYI] Parameters.FreeRenderableObjectIndex" );
      //Parameters.tboObjectsMatrices.FreeIndex( index );
    }
  }
}


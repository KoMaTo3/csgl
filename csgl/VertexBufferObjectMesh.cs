using System;
using System.Collections.Generic;

namespace csgl {
  public class VertexBufferObjectMesh {
    private int _hash;
    private Queue< UInt32 > _indicesList;

    public int hash {
      get { return this._hash; }
      private set { this._hash = value; }
    }

    private VertexBufferObjectMesh() {
    }

    public VertexBufferObjectMesh( Mesh mesh, UInt32 offset ) {
      this._indicesList = new Queue< UInt32 >();
      this.hash = mesh.hash;
      foreach( UInt32 index in mesh.indices ) {
        this._indicesList.Enqueue( index + offset );
      }
    }
  }
}


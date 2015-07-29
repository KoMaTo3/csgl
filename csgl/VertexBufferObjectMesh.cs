using System;
using System.Collections.Generic;

namespace csgl {
  public class VertexBufferObjectMesh {
    private int _hash;
    private Queue< uint > _indicesList;
    private UInt32 _offset;

    public int hash {
      get { return this._hash; }
      private set { this._hash = value; }
    }

    public UInt32 offset {
      get { return this._offset; }
      private set { this._offset = value; }
    }

    public Queue< uint > indicesList {
      get { return this._indicesList; }
      private set { this._indicesList = value; }
    }

    private VertexBufferObjectMesh() {
    }

    public VertexBufferObjectMesh( Mesh mesh, UInt32 offset ) {
      this._offset = offset;
      this._indicesList = new Queue< UInt32 >();
      this.hash = mesh.hash;
      foreach( UInt32 index in mesh.indices ) {
        this._indicesList.Enqueue( index + offset );
      }
    }
  }
}


using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace csgl {
  public class Object: IDisposable {
    private Material _material;
    private bool _isValid;
    private string name;
    private Vector2 _position;
    private Vector2 _scale;
    private Mesh _mesh;
    private VertexBufferObject _vbo;

    public bool isValid {
      get { return this._isValid; }
      private set { this._isValid = value; }
    }

    public Vector2 position {
      get { return this._position; }
      set { this._position = value; }
    }

    public Vector2 scale {
      get { return this._scale; }
      set { this._scale = value; }
    }

    public Mesh mesh {
      get { return this._mesh; }
      set {
        if( this._mesh != null ) {
          this._mesh.Dec();
        }
        this._mesh = value;
        if( this._mesh != null ) {
          this._mesh.Inc();
          this._vbo.AddMesh( this._mesh );
        }
      }
    }

    private Object() {
    }

    public Material material {
      get { return this._material; }
      set {
        if( this._material != null ) {
          this._material.Dec();
        }
        this._material = value;
        if( this._material != null ) {
          this._material.Inc();
        }
      }
    }

    public Object( string setName ) {
      this.name = setName;
      this.isValid = true;
      this.scale = new Vector2( 0.0f, 0.0f );
      this.position = new Vector2( 0.0f, 0.0f );
      this._vbo = new VertexBufferObject();
    }

    ~Object() {
      this.Dispose( true );
    }

    public void Dispose() {
      this.Dispose( true );
      GC.SuppressFinalize( this );
    }

    public void Render() {
      if( this.material == null ) {
        Console.WriteLine( "[ERROR] object '{0}' don't have material", this.name );
        return;
      }

      if( this.mesh == null ) {
        Console.WriteLine( "[ERROR] object '{0}' don't have mesh", this.name );
        return;
      }

      this.material.Apply();

      this._vbo.BindVertexBuffer();
      this._vbo.BindColorBuffer();
      this._vbo.BindPerVertexInfoBuffer();
      GL.DrawArrays( PrimitiveType.TriangleStrip, 0, 4 );
    }

    protected virtual void Dispose( bool disposing ) {
      if( disposing ) {
        if( this.isValid ) {
          Console.WriteLine( "~Object {0}", this.name );
          this.isValid = false;
          this.material = null;
          this._vbo.Dispose();
          Console.WriteLine( "~Object {0} done", this.name );
        }
      }
    }

    public override String ToString() {
      return string.Format( "%s",
        this.material == null ? "" : this.material.name
      );
    }
  }
}


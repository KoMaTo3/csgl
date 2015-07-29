using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace csgl {
  public class Object: IDisposable {
    private Material _material;
    private bool _isValid;
    private string _name;
    private Vector3 _position;
    private Vector3 _scale;
    private Vector3 _rotation;
    private Mesh _mesh;
    private VertexBufferObject _vbo;
    private UInt32 _renderableObjectIndex = 0;
    private bool _changed = true;
    private VertexBufferObjectMesh vboMesh = null;
    //индекс объекта в массиве объектных матриц

    public string name {
      get { return this._name; }
      private set { this._name = value; }
    }

    public bool isValid {
      get { return this._isValid; }
      private set { this._isValid = value; }
    }

    public Vector3 position {
      get { return this._position; }
      set {
        if( this._position != value ) {
          this._changed = true;
        }
        this._position = value;
      }
    }

    public Vector3 scale {
      get { return this._scale; }
      set {
        if( this._scale != value ) {
          this._changed = true;
        }
        this._scale = value;
      }
    }

    public Vector3 rotation {
      get { return this._rotation; }
      set {
        if( this._rotation != value ) {
          this._changed = true;
        }
        this._rotation = value;
      }
    }

    public Mesh mesh {
      get { return this._mesh; }
      set {
        if( this._mesh != null ) {
          this._mesh.Dec();
        }
        this._mesh = value;
        if( this._renderableObjectIndex == 0 ) {
          this._renderableObjectIndex = Parameters.GetRenderableObjectIndex();
        }
        if( this._mesh != null ) {
          this._mesh.Inc();
          this.vboMesh = this._vbo.AddMesh( this._mesh, this._renderableObjectIndex );
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
      this.scale = Vector3.One;
      this.position = Vector3.Zero;
      this.rotation = Vector3.Zero;
      this._vbo = new VertexBufferObject();
    }

    ~Object() {
      this.Dispose( true );
    }

    public void Dispose() {
      this.Dispose( true );
      GC.SuppressFinalize( this );
    }

    public void Update() {
      if( this._changed ) {
        Matrix4 matrix = Tools.MakeMatrix(
                           new Vector3( this.position.X, this.position.Y, this.position.Z ),
                           new Vector3( this.scale.X, this.scale.Y, this.scale.Z ),
                           this.rotation
                         );
        Matrix4 matrixRotation = Tools.MakeMatrixRotation( -this.rotation );
        Parameters.tboObjectsMatrices.SetData(
          matrix,
          matrixRotation,
          this._renderableObjectIndex
        );
        this._changed = false;
      }
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

      Console.WriteLine( "Render[{0}] material[{1}]", this.name, this.material.ToString() );
      Console.WriteLine( "> before this.material.Apply: {0}", GL.GetError() );
      this.material.Apply();
      this.material.shader.Use();
      Console.WriteLine( "> after this.material.Apply: {0}", GL.GetError() );

      this._vbo.BindVertexBuffer();
      Console.WriteLine( "> BindVertexBuffer: {0}", GL.GetError() );
      this._vbo.BindColorBuffer();
      Console.WriteLine( "> BindColorBuffer: {0}", GL.GetError() );
      this._vbo.BindPerVertexInfoBuffer();
      Console.WriteLine( "> BindPerVertexInfoBuffer: {0}", GL.GetError() );
      this._vbo.BindTexCoordsBuffer();
      Console.WriteLine( "> BindTexCoordsBuffer: {0}", GL.GetError() );
      this._vbo.BindNormalsBuffer();
      Console.WriteLine( "> BindNormalsBuffer: {0}", GL.GetError() );
      this._vbo.BindIndexBuffer();
      Console.WriteLine( "> BindIndexBuffer: {0}", GL.GetError() );
      //GL.DrawArrays( PrimitiveType.TriangleStrip, ( int ) this.vboMesh.offset, this.mesh.indices.Count );
      GL.DrawElements( BeginMode.TriangleStrip, this.mesh.indices.Count, DrawElementsType.UnsignedInt, 0 );
      Console.WriteLine( "> DrawElements: {0}", GL.GetError() );
    }

    protected virtual void Dispose( bool disposing ) {
      if( disposing ) {
        if( this.isValid ) {
          Console.WriteLine( "~Object {0}", this.name );
          this.isValid = false;
          this.material = null;
          this._vbo.Dispose();
          if( this._renderableObjectIndex > 0 ) {
            Parameters.FreeRenderableObjectIndex( this._renderableObjectIndex );
            this._renderableObjectIndex = 0;
          }
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


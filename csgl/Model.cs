using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

/*
 * model = mesh + material
*/

namespace csgl {
  public class Model: IDisposable, IResourceEntity {
    public Resource resource;
    private bool _isValid;
    private string _name;
    private Mesh _mesh;
    private Material _material;

    public string name {
      get{ return this._name; }
      private set{ this._name = value; }
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
        }
      }
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

    public bool isValid {
      get { return this._isValid; }
      private set { this._isValid = value; }
    }

    public void Inc() {
      if( this.resource != null ) {
        this.resource.Inc();
      }
    }

    public void Dec() {
      if( this.resource != null ) {
        this.resource.Dec();
      }
    }

    private Model() {
      this.isValid = false;
    }

    public Model( string setName ) {
      this.isValid = true;
      this.name = setName;
    }

    ~Model() {
      this.Dispose( true );
    }

    public void Dispose() {
      this.Dispose( true );
      GC.SuppressFinalize( this );
    }

    protected virtual void Dispose( bool disposing ) {
      if( disposing ) {
        if( this.isValid ) {
          Console.WriteLine( "~Model {0}", this.name );
          this.isValid = false;
          if( this.resource != null ) {
            this.resource.Dispose();
            this.resource = null;
            if( this.mesh != null ) {
              this.mesh.Dec();
            }
            this.mesh = null;
            if( this.material != null ) {
              this.material.Dec();
            }
            this.material = null;
          }
          Console.WriteLine( "~Model {0} done", this.name );
        }
      }
    }
  }
}


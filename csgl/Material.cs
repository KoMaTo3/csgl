using System;

/*
 * material = shader + textures //? + attributes
*/

namespace csgl {
  public class Material: IDisposable, IResourceEntity {
    public Resource resource;
    private bool _isValid;
    private string _name;
    private ShaderProgram _shader;

    public string name {
      get{ return this._name; }
      private set{ this._name = value; }
    }

    public ShaderProgram shader {
      get { return this._shader; }
      set {
        if( this._shader != null ) {
          this._shader.Dec();
        }
        this._shader = value;
        if( this._shader != null ) {
          this._shader.Inc();
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

    private Material() {
      this.isValid = false;
    }

    public Material( string setName ) {
      this.isValid = true;
      this.name = setName;
    }

    ~Material() {
      this.Dispose( true );
    }

    public void Dispose() {
      this.Dispose( true );
      GC.SuppressFinalize( this );
    }

    public void Apply() {
      if( this.shader != null ) {
        this.shader.Use();
      }
    }

    protected virtual void Dispose( bool disposing ) {
      if( disposing ) {
        if( this.isValid ) {
          Console.WriteLine( "~Material {0}", this.name );
          this.isValid = false;
          if( this.resource != null ) {
            this.resource.Dispose();
            this.resource = null;
            if( this.shader != null ) {
              this.shader.Dec();
            }
            this.shader = null;
          }
          Console.WriteLine( "~Material {0} done", this.name );
        }
      }
    }
  }
}


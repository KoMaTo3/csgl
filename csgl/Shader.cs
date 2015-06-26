using System;
using System.Diagnostics;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace csgl {
  public class Shader: IDisposable, IResourceEntity {
    public Resource resource;
    private int _descriptor;
    private string _name;

    public int descriptor {
      get { return this._descriptor; }
      private set { this.descriptor = value; }
    }

    private Shader() {
      this._descriptor = 0;
    }

    public string name {
      get{ return this._name; }
      private set{ this._name = value; }
    }

    public Shader( string setName, string source, ShaderType type ) {
      this._descriptor = GL.CreateShader( type );
      this.name = setName;
      GL.ShaderSource( this.descriptor, source );
      GL.CompileShader( this.descriptor );
      string log = GL.GetShaderInfoLog( this.descriptor );
      if( log.Length > 0 ) {
        Console.WriteLine( "Shader error: name[{0}] type[{1}] description[{2}] source[{3}]", this.name, type, log, source );
      }
    }

    ~Shader() {
      this.Dispose( true );
    }

    public void Dispose() {
      this.Dispose( true );
      GC.SuppressFinalize( this );
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

    protected virtual void Dispose( bool disposing ) {
      if( disposing ) {
        if( this.descriptor != 0 ) {
          Console.WriteLine( "~Shader {0}", this.name );
          GL.DeleteShader( this.descriptor );
          this._descriptor = 0;
          if( this.resource != null ) {
            this.resource.Dispose();
            this.resource = null;
          }
          Console.WriteLine( "~Shader {0} done", this.name );
        }
      }
    }
  }
}


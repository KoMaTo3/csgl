using System;
using OpenTK.Graphics.OpenGL;
using csgl;

namespace csgl {
  public class ResourceShader: Resource, IDisposable {
    public Shader shader {
      get { return this._shader; }
      protected set { this._shader = value; }
    }

    protected Shader _shader;

    private ResourceShader()
      : base( "" ) {
    }

    public ResourceShader( string name, string source, ShaderType shaderType )
      : base( name ) {
      this.type = ResourceType.SHADER;
      this.shader = new Shader( name, source, shaderType );
      this.shader.resource = this;
      this.IsValid = true;
    }

    ~ResourceShader() {
      this.Dispose( true );
    }

    protected override void Dispose( bool disposing ) {
      if( disposing ) {
        if( this.IsValid ) {
          Console.WriteLine( "~ResourceShader '{0}'", this.name );
          Resource.RemoveResource( this.id );
          this.IsValid = false;
          this.shader.Dispose();
          Console.WriteLine( "~ResourceShader '{0}' done", this.name );
        }
      }
    }

    public static explicit operator Shader( ResourceShader resource ) {
      return resource.shader;
    }
  }
}


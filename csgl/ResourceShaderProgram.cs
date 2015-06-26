using System;
using OpenTK.Graphics.OpenGL;
using csgl;

namespace csgl {
  public class ResourceShaderProgram: Resource {
    public ShaderProgram shaderProgram {
      get { return this._shaderProgram; }
      protected set { this._shaderProgram = value; }
    }

    protected ShaderProgram _shaderProgram;

    public ResourceShaderProgram()
      : base( "" ) {
    }

    public ResourceShaderProgram( string name, string source )
      : base( name ) {
      this.type = ResourceType.SHADER_PROGRAM;
      this.shaderProgram = new ShaderProgram( name );
      this.shaderProgram.resource = this;
      this.shaderProgram.Make( source.Split( '\n' ) );
      this.IsValid = true;
    }

    ~ResourceShaderProgram() {
      this.Dispose( true );
    }

    protected override void Dispose( bool disposing ) {
      if( disposing ) {
        if( this.IsValid ) {
          Console.WriteLine( "~ResourceShaderProgram '{0}'", this.name );
          Resource.RemoveResource( this.id );
          this.IsValid = false;
          this.shaderProgram.Dispose();
          Console.WriteLine( "~ResourceShaderProgram '{0}' done", this.name );
        }
      }
    }

    public static explicit operator ShaderProgram( ResourceShaderProgram resource ) {
      if( resource == null || resource.type != ResourceType.SHADER_PROGRAM ) {
        Console.WriteLine( "[ERROR] Cast from {0} to shader program", resource == null ? ResourceType.NULL : resource.type );
      }

      return resource.shaderProgram;
    }
  }
}


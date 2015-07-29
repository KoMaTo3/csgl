using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace csgl {
  public class ShaderProgramUniformTexture: ShaderProgramUniform {
    private Texture _texture;

    public Texture texture {
      get { return this._texture; }
      protected set { this._texture = value; }
    }

    public override void Apply() {
      Console.WriteLine( "sp uniform texture {0} = {1}", this.name, this._texture.descriptor );
      GL.Uniform1( this.location, this._texture.descriptor );
    }

    private ShaderProgramUniformTexture()
      : base( "" ) {
      this.location = -1;
    }

    public ShaderProgramUniformTexture( string setName, Texture setTexture )
      : base( setName ) {
      this.texture = setTexture;
    }
  }
}


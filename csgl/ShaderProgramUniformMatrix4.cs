using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace csgl {
  public class ShaderProgramUniformMatrix4: ShaderProgramUniform {
    private Matrix4 _matrix;

    public Matrix4 matrix {
      get { return this._matrix; }
      protected set { this._matrix = value; }
    }

    public override void Apply() {
      GL.UniformMatrix4( this.location, false, ref this._matrix );
    }

    private ShaderProgramUniformMatrix4()
      : base( "" ) {
      this.location = -1;
    }

    public ShaderProgramUniformMatrix4( string setName, Matrix4 setMatrix )
      : base( setName ) {
      this.matrix = setMatrix;
    }
  }
}


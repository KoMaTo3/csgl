using System;
using OpenTK.Graphics.OpenGL;

namespace csgl {
  public abstract class ShaderProgramUniform: IShaderProgramUniform {
    private string _name;
    protected int location = -1;

    public string name {
      get { return this._name; }
      set { this._name = value; }
    }

    public abstract void Apply();

    private ShaderProgramUniform() {
    }

    public ShaderProgramUniform( string setName ) {
      this.name = setName;
    }

    public bool AttachToShaderProgram( int shaderProgram ) {
      this.location = GL.GetUniformLocation( shaderProgram, this.name );
      return ( this.location != -1 );
    }
  }
}


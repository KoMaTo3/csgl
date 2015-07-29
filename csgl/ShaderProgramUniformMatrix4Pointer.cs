using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace csgl {
  public class ShaderProgramUniformMatrix4Pointer: ShaderProgramUniform {
    private ValueMatrix4 _value;

    public Matrix4 value {
      get { return this._value.value; }
      protected set { }
    }

    public override void Apply() {
      GL.UniformMatrix4( this.location, false, ref this._value.value );
    }

    private ShaderProgramUniformMatrix4Pointer()
      : base( "" ) {
      this.location = -1;
    }

    public ShaderProgramUniformMatrix4Pointer( string setName, ValueMatrix4 setValue )
      : base( setName ) {
      this._value = setValue;
    }
  }
}


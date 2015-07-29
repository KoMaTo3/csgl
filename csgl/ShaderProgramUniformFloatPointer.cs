using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace csgl {
  public class ShaderProgramUniformFloatPointer: ShaderProgramUniform {
    private ValueFloat _value;

    public float value {
      get { return this._value.value; }
      protected set { }
    }

    public override void Apply() {
      GL.Uniform1( this.location, this._value.value );
    }

    private ShaderProgramUniformFloatPointer()
      : base( "" ) {
      this.location = -1;
    }

    public ShaderProgramUniformFloatPointer( string setName, ValueFloat setValue )
      : base( setName ) {
      this._value = setValue;
    }
  }
}


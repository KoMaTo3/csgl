using System;

namespace csgl {
  public interface IShaderProgramUniform {
    string name {
      get;
      set;
    }

    void Apply();

    bool AttachToShaderProgram( int shaderProgram );
  }
}


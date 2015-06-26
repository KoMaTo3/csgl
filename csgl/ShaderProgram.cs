using System;
using System.Diagnostics;
using System.Collections.Generic;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Collections.ObjectModel;

namespace csgl {
  public class ShaderProgram: IDisposable, IResourceEntity {
    static protected int _activeShaderProgram = 0;

    static public int activeShaderProgram {
      get { return ShaderProgram._activeShaderProgram; }
      protected set { ShaderProgram._activeShaderProgram = value; }
    }

    public Resource resource;
    private int _descriptor;
    private Queue< Shader > shadersList;
    private string _name;
    protected Queue< IShaderProgramUniform > uniformsList;

    public int descriptor {
      get { return this._descriptor; }
      private set { this._descriptor = value; }
    }

    public string name {
      get{ return this._name; }
      private set{ this._name = value; }
    }

    private ShaderProgram() {
    }

    public ShaderProgram( string setName ) {
      this.shadersList = new Queue< Shader >();
      this.descriptor = GL.CreateProgram();
      this.name = setName;
      this.uniformsList = new Queue< IShaderProgramUniform >();
    }

    ~ShaderProgram() {
      this.Dispose( true );
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

    public void Dispose() {
      this.Dispose( true );
      GC.SuppressFinalize( this );
    }

    public void AddUniform( IShaderProgramUniform uniform ) {
      if( uniform.AttachToShaderProgram( this.descriptor ) ) {
        this.uniformsList.Enqueue( uniform );
      } else {
        Console.WriteLine( "[WARNING] ShaderProgram.AddUniform => '{0}' not found in shader program '{1}'", uniform.name, this.name );
      }
    }

    protected virtual void Dispose( bool disposing ) {
      if( disposing ) {
        if( this.descriptor != 0 ) {
          Console.WriteLine( "~ShaderProgram" );
          GL.DeleteProgram( this.descriptor );
          this.descriptor = 0;
          if( this.resource != null ) {
            this.resource.Dispose();
            this.resource = null;
          }
          this.uniformsList.Clear();
          this.uniformsList = null;
          Console.WriteLine( "~ShaderProgram done" );
        }
      }
    }

    public bool AttachShader( Shader shader, bool autoLink = true ) {
      GL.AttachShader( this.descriptor, shader.descriptor );
      this.shadersList.Enqueue( shader );
      if( autoLink ) {
        this.LinkProgram();
      }

      return true;
    }

    public bool LinkProgram() {
      GL.LinkProgram( this.descriptor );
      string log = GL.GetProgramInfoLog( this.descriptor );
      if( log.Length > 0 ) {
        Console.WriteLine( "Shader program error: id[{0}] name[{1}]", this.descriptor, this.name );
      }

      foreach( var shader in this.shadersList ) {
        GL.DetachShader( this.descriptor, shader.descriptor );
      }
      this.shadersList.Clear();

      return true;
    }

    public void Make( string [] shadersList ) {
      Queue<Resource> resourceList = new Queue<Resource>();
      foreach( string fileName in shadersList ) {
        Resource res = Resource.Get( fileName );
        Shader shader = ( Shader ) ( ResourceShader ) res;
        this.AttachShader( shader, false );
        resourceList.Enqueue( res );
      }
      this.LinkProgram();
      foreach( Resource res in resourceList ) {
        res.Dispose();
      }
    }

    public void Use() {
      bool setUniforms = ( ShaderProgram.activeShaderProgram != this.descriptor );
      GL.UseProgram( this.descriptor );
      ShaderProgram.activeShaderProgram = this.descriptor;
      if( setUniforms ) {
        foreach( ShaderProgramUniform uniform in this.uniformsList ) {
          uniform.Apply();
        }
      }
    }

    public static void UnUse() {
      GL.UseProgram( 0 );
      ShaderProgram.activeShaderProgram = 0;
    }
  }
}


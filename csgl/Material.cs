using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

/*
 * material = shader + textures //? + attributes
*/

namespace csgl {
  public class Material: IDisposable, IResourceEntity {
    public Resource resource;
    private bool _isValid;
    private string _name;
    private ShaderProgram _shader;
    protected Dictionary< string, Texture > texturesList;

    public string name {
      get{ return this._name; }
      private set{ this._name = value; }
    }

    public ShaderProgram shader {
      get { return this._shader; }
      set {
        if( this._shader != null ) {
          this._shader.Dec();
        }
        this._shader = value;
        if( this._shader != null ) {
          this._shader.Inc();
        }
      }
    }

    public bool isValid {
      get { return this._isValid; }
      private set { this._isValid = value; }
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

    private Material() {
      this.isValid = false;
    }

    public Material( string setName ) {
      this.isValid = true;
      this.name = setName;
      this.texturesList = new Dictionary< string, Texture >();
    }

    public void AddTexture( string name, Texture texture ) {
      this.texturesList.Add( name, texture );
    }

    ~Material() {
      this.Dispose( true );
    }

    public void Dispose() {
      this.Dispose( true );
      GC.SuppressFinalize( this );
    }

    public void Apply() {
      if( this.shader != null ) {
        Console.WriteLine( "> before this.shader.Use(): {0}", GL.GetError() );
        this.shader.Use();
        int textureNum = 0;
        /* for( int q = 0; q < this.texturesList.Count; ++q ) {
          GL.ActiveTexture( TextureUnit.Texture0 + q );
          Texture.BindNull();
        } */
        Console.WriteLine( "mat apply => textures {0}", this.texturesList.Count );
        foreach( var texture in this.texturesList ) {
          //GL.ActiveTexture( TextureUnit.Texture0 + texture.Value.descriptor );
          //texture.Value.Bind();
          //GL.ActiveTexture( TextureUnit.Texture0 + textureNum );
          //GL.Uniform1( this.shader.GetUniformLocation( texture.Key ), texture.Value.descriptor );
          //Console.WriteLine( "material apply => {0} = {1}",  );
          Console.WriteLine( "bind texture '{0}' uniform[{1}] textureNum[{2}]", texture.Key, this.shader.GetUniformLocation( texture.Key ), textureNum );
          this.shader.SetUniform( texture.Key, textureNum );
          Console.WriteLine( "> this.shader.SetUniform: {0}", GL.GetError() );
          GL.ActiveTexture( TextureUnit.Texture0 + textureNum );
          Console.WriteLine( "> GL.ActiveTexture: {0}", GL.GetError() );
          texture.Value.Bind();
          //GL.BindSampler( textureNum,  );
          textureNum += 1;
        }
      } else {
        Console.WriteLine( "[WARNING] Material.Apply => shader is null" );
      }
    }

    protected virtual void Dispose( bool disposing ) {
      if( disposing ) {
        if( this.isValid ) {
          Console.WriteLine( "~Material {0}", this.name );
          this.isValid = false;
          foreach( var texture in this.texturesList ) {
            if( texture.Value.resource != null ) {
              texture.Value.resource.Dec();
            }
          }
          if( this.resource != null ) {
            this.resource.Dispose();
            this.resource = null;
            if( this.shader != null ) {
              this.shader.Dec();
            }
            this.shader = null;
          }
          Console.WriteLine( "~Material {0} done", this.name );
        }
      }
    }

    public override String ToString() {
      return string.Format( "{0}:{1},t{2}", this.name, this.shader.name, this.texturesList.Count );
    }
  }
}


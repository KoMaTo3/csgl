using System;
using System.Collections;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace csgl {
  public enum TextureType {
    TEXTURE0,
    TEXTURE1,
    TEXTURE2,
    TEXTURE3,
  };

  public class Texture: IDisposable {
    public Resource resource;
    private string name;
    private int _descriptor;
    public static Dictionary< TextureType, string > TextureType2uniformName = new Dictionary< TextureType, string >();

    public int descriptor {
      get { return this._descriptor; }
      private set { this.descriptor = value; }
    }

    private Texture() {
      this._descriptor = 0;
    }

    public unsafe Texture( string setName, Image image ) {
      this.name = setName;
      this._descriptor = GL.GenTexture();
      GL.BindTexture( TextureTarget.Texture2D, this.descriptor );
      GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ( int ) TextureMagFilter.Nearest );
      GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ( int ) TextureMinFilter.Nearest );
      fixed( byte *rawImage = image.GetRawPointer() ) {
        GL.TexImage2D( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.width, image.height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ( IntPtr ) rawImage );
      }
      GL.BindTexture( TextureTarget.Texture2D, 0 );
    }

    public void Bind( TextureTarget target = TextureTarget.Texture2D ) {
      Console.WriteLine( "bind texture {0} = {1}", this.name, this.descriptor );
      GL.BindTexture( target, this.descriptor );
    }

    static public void BindNull( TextureTarget target = TextureTarget.Texture2D ) {
      GL.BindTexture( target, 0 );
    }

    ~Texture() {
      this.Dispose( true );
    }

    public void Dispose() {
      this.Dispose( true );
      GC.SuppressFinalize( this );
    }

    protected virtual void Dispose( bool disposing ) {
      if( disposing ) {
        if( this.descriptor != 0 ) {
          Console.WriteLine( "~Texture {0}", this.name );
          GL.DeleteTexture( this.descriptor );
          this._descriptor = 0;
          if( this.resource != null ) {
            this.resource.Dispose();
            this.resource = null;
          }
          Console.WriteLine( "~Texture {0} done", this.name );
        }
      }
    }

    public override String ToString() {
      return string.Format( "texture:%s", this.name );
    }
  }
}


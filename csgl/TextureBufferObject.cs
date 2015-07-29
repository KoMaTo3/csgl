using System;
using System.Collections;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Linq;
using System.Runtime.InteropServices;
using Pango;

namespace csgl {
  public unsafe class TextureBufferObject: IDisposable {
    private int _bufferDescriptor = 0;
    private int _textureDescriptor = 0;
    private string _uniformName;
    private UInt32 elementSize;
    private UInt32 elementsCount = 0;
    private Matrix4 [] elementsList;
    const UInt32 matricesPerElement = 2;

    public int bufferDescriptor {
      get { return this._bufferDescriptor; }
      private set { this._bufferDescriptor = value; }
    }

    public string uniformName {
      get { return this._uniformName; }
      private set { this._uniformName = value; }
    }

    private TextureBufferObject() {
    }

    public TextureBufferObject( string setUniformName ) {
      this._bufferDescriptor = GL.GenBuffer();
      this._textureDescriptor = GL.GenTexture();
      GL.BindBuffer( BufferTarget.TextureBuffer, this._bufferDescriptor );
      GL.TexBuffer( TextureBufferTarget.TextureBuffer, SizedInternalFormat.R32f, this._bufferDescriptor );
      this.elementSize = ( UInt32 ) sizeof( Matrix4 );
      this.elementsList = new Matrix4[ 0 ];
    }

    /* public void SetData( IntPtr data ) {
      GL.BindBuffer( BufferTarget.TextureBuffer, this._bufferDescriptor );
      GL.BufferData( BufferTarget.TextureBuffer, ( IntPtr ) ( this.elementSize * this.elementsCount ), data, BufferUsageHint.StaticDraw );
      GL.BindBuffer( BufferTarget.TextureBuffer, 0 );
      GL.TexBuffer( TextureBufferTarget.TextureBuffer, SizedInternalFormat.R32f, this._bufferDescriptor );
    } */

    public unsafe void SetData( Matrix4 objectMatrix, Matrix4 rotationMatrix, UInt32 index ) {
      this.elementsList[ index * matricesPerElement ] = objectMatrix;
      this.elementsList[ index * matricesPerElement + 1 ] = rotationMatrix;
      GL.BindBuffer( BufferTarget.TextureBuffer, this._bufferDescriptor );
      fixed( Matrix4* pMatrixArray = this.elementsList ) {
        GL.BufferSubData( BufferTarget.TextureBuffer, ( IntPtr ) ( ( index * matricesPerElement ) * this.elementSize ), ( IntPtr ) ( this.elementSize * matricesPerElement ), ( IntPtr ) ( pMatrixArray + ( index * matricesPerElement ) ) );
      }

      //GL.BindBuffer( BufferTarget.TextureBuffer, 0 );
      GL.TexBuffer( TextureBufferTarget.TextureBuffer, SizedInternalFormat.R32f, this._bufferDescriptor );
    }

    public UInt32 GetFreeIndex() {
      ++this.elementsCount;
      this.elementsList = this.elementsList.Concat( new Matrix4[ 2 ] ).ToArray();

      //resize buffer
      GL.BindBuffer( BufferTarget.TextureBuffer, this._bufferDescriptor );
      GL.BufferData< Matrix4 >( BufferTarget.TextureBuffer, ( IntPtr ) ( ( this.elementsCount * matricesPerElement ) * this.elementSize ), this.elementsList, BufferUsageHint.StaticDraw );
      //GL.BindBuffer( BufferTarget.TextureBuffer, 0 );

      return this.elementsCount - 1;
    }

    public void Bind( ShaderProgram shaderProgram ) {
      GL.Uniform1( shaderProgram.GetUniformLocation( this._uniformName ), this._textureDescriptor );
    }

    ~TextureBufferObject() {
      this.Dispose( true );
    }

    public void Dispose() {
      this.Dispose( true );
      GC.SuppressFinalize( this );
    }

    protected virtual void Dispose( bool disposing ) {
      if( disposing ) {
        if( this._bufferDescriptor != 0 ) {
          Console.WriteLine( "~TextureBufferObject" );
          GL.DeleteBuffer( this._bufferDescriptor );
          GL.DeleteTexture( this._textureDescriptor );
          this._bufferDescriptor = 0;
          this._textureDescriptor = 0;
          Console.WriteLine( "~TextureBufferObject done" );
        }
      }
    }
  }
}


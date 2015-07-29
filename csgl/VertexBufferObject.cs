using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

/*
 * Хранит в себе наборы мешей с пересчитанными индексами
*/

namespace csgl {
  public class VertexBufferObject: IDisposable {
    private int [] _descriptors;
    private int _ibo;
    //координаты вершин
    private Queue< Vector3 > _verticesList;
    //цвета вершин
    private Queue< Vector4 > _colorsList;
    //доп. инфа по вершинам: x - индекс матрицы
    //текстурные координаты + 2 свободных float
    private Queue< Vector4 > _texCoordsList;
    private Queue< Vector4UInt32 > _perVerticeData;
    private Queue< Vector3 > _normalsList;
    private List< VertexBufferObjectMesh > _meshList;
    const int DESCRIPTORS_COUNT = 5;
    const int DESCRIPTOR_VERTEX = 0;
    const int DESCRIPTOR_COLOR = 1;
    const int DESCRIPTOR_INFO = 2;
    const int DESCRIPTOR_TEXCOORDS = 3;
    const int DESCRIPTOR_NORMALS = 4;

    public int [] descriptors {
      get { return this._descriptors; }
      private set { this._descriptors = value; }
    }

    public VertexBufferObjectMesh MeshExists( Mesh mesh ) {
      foreach( VertexBufferObjectMesh thisMesh in this._meshList ) {
        if( thisMesh.hash == mesh.hash ) {
          return thisMesh;
        }
      }
      return null;
    }

    public UInt32 GetFreeMeshIndex() {
      return ( UInt32 ) this._meshList.Count;
    }

    public VertexBufferObjectMesh AddMesh( Mesh mesh, UInt32 renderableObjectIndex ) {
      VertexBufferObjectMesh existsMesh = this.MeshExists( mesh );
      if( existsMesh != null ) {
        return existsMesh;
      }

      UInt32 offset = ( UInt32 ) this._verticesList.Count;
      foreach( var vertex in mesh.vertices ) {
        this._verticesList.Enqueue( vertex );
        this._perVerticeData.Enqueue( new Vector4UInt32( renderableObjectIndex, 0, 0, 0 ) );
        this._perVerticeData.Enqueue( new Vector4UInt32( renderableObjectIndex, 0, 0, 0 ) );
        this._perVerticeData.Enqueue( new Vector4UInt32( renderableObjectIndex, 0, 0, 0 ) );
        this._perVerticeData.Enqueue( new Vector4UInt32( renderableObjectIndex, 0, 0, 0 ) );
      }
      foreach( var color in mesh.colors ) {
        this._colorsList.Enqueue( color );
      }
      foreach( var normal in mesh.normals ) {
        this._normalsList.Enqueue( normal );
      }
      foreach( var texCoord in mesh.texCoords ) {
        this._texCoordsList.Enqueue( new Vector4( texCoord.X, texCoord.Y, 0.0f, 0.0f ) );
      }
      VertexBufferObjectMesh newVBOMesh = new VertexBufferObjectMesh( mesh, offset );
      this._meshList.Add( newVBOMesh );

      GL.BindBuffer( BufferTarget.ElementArrayBuffer, this._ibo );
      GL.BufferData( BufferTarget.ElementArrayBuffer, ( IntPtr ) ( sizeof( uint ) * mesh.indices.Count ), newVBOMesh.indicesList.ToArray(), BufferUsageHint.StaticDraw );

      GL.BindBuffer( BufferTarget.ArrayBuffer, this.descriptors[ VertexBufferObject.DESCRIPTOR_VERTEX ] );
      GL.BufferData( BufferTarget.ArrayBuffer, ( IntPtr ) ( this._verticesList.Count * sizeof( float ) * 3 ), this._verticesList.ToArray(), BufferUsageHint.StaticDraw );

      GL.BindBuffer( BufferTarget.ArrayBuffer, this.descriptors[ VertexBufferObject.DESCRIPTOR_COLOR ] );
      GL.BufferData( BufferTarget.ArrayBuffer, ( IntPtr ) ( this._colorsList.Count * sizeof( float ) * 4 ), this._colorsList.ToArray(), BufferUsageHint.StaticDraw );

      GL.BindBuffer( BufferTarget.ArrayBuffer, this.descriptors[ VertexBufferObject.DESCRIPTOR_INFO ] );
      GL.BufferData( BufferTarget.ArrayBuffer, ( IntPtr ) ( this._perVerticeData.Count * sizeof( UInt32 ) * 4 ), this._perVerticeData.ToArray(), BufferUsageHint.StaticDraw );

      GL.BindBuffer( BufferTarget.ArrayBuffer, this.descriptors[ VertexBufferObject.DESCRIPTOR_TEXCOORDS ] );
      GL.BufferData( BufferTarget.ArrayBuffer, ( IntPtr ) ( this._texCoordsList.Count * sizeof( float ) * 4 ), this._texCoordsList.ToArray(), BufferUsageHint.StaticDraw );

      GL.BindBuffer( BufferTarget.ArrayBuffer, this.descriptors[ VertexBufferObject.DESCRIPTOR_NORMALS ] );
      GL.BufferData( BufferTarget.ArrayBuffer, ( IntPtr ) ( this._normalsList.Count * sizeof( float ) * 3 ), this._normalsList.ToArray(), BufferUsageHint.StaticDraw );

      GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );

      return newVBOMesh;
    }

    public void BindVertexBuffer( int index = VertexBufferObject.DESCRIPTOR_VERTEX ) {
      GL.BindBuffer( BufferTarget.ArrayBuffer, this.descriptors[ VertexBufferObject.DESCRIPTOR_VERTEX ] );
      GL.VertexAttribPointer( index, 3, VertexAttribPointerType.Float, false, 0, 0 );
      GL.EnableVertexAttribArray( index );
    }

    public void BindColorBuffer( int index = VertexBufferObject.DESCRIPTOR_COLOR ) {
      GL.BindBuffer( BufferTarget.ArrayBuffer, this.descriptors[ VertexBufferObject.DESCRIPTOR_COLOR ] );
      GL.VertexAttribPointer( index, 4, VertexAttribPointerType.Float, false, 0, 0 );
      GL.EnableVertexAttribArray( index );
    }

    public void BindPerVertexInfoBuffer( int index = VertexBufferObject.DESCRIPTOR_INFO ) {
      GL.BindBuffer( BufferTarget.ArrayBuffer, this.descriptors[ VertexBufferObject.DESCRIPTOR_INFO ] );
      GL.VertexAttribPointer( index, 4, VertexAttribPointerType.UnsignedInt, false, 0, 0 );
      GL.EnableVertexAttribArray( index );
    }

    public void BindTexCoordsBuffer( int index = VertexBufferObject.DESCRIPTOR_TEXCOORDS ) {
      GL.BindBuffer( BufferTarget.ArrayBuffer, this.descriptors[ VertexBufferObject.DESCRIPTOR_TEXCOORDS ] );
      GL.VertexAttribPointer( index, 4, VertexAttribPointerType.Float, false, 0, 0 );
      GL.EnableVertexAttribArray( index );
    }

    public void BindNormalsBuffer( int index = VertexBufferObject.DESCRIPTOR_NORMALS ) {
      GL.BindBuffer( BufferTarget.ArrayBuffer, this.descriptors[ VertexBufferObject.DESCRIPTOR_NORMALS ] );
      GL.VertexAttribPointer( index, 3, VertexAttribPointerType.Float, false, 0, 0 );
      GL.EnableVertexAttribArray( index );
    }

    public void BindIndexBuffer() {
      GL.BindBuffer( BufferTarget.ElementArrayBuffer, this._ibo );
    }

    public VertexBufferObject() {
      this.descriptors = new int[ VertexBufferObject.DESCRIPTORS_COUNT ];
      GL.GenBuffers( VertexBufferObject.DESCRIPTORS_COUNT, this.descriptors );
      this._meshList = new List< VertexBufferObjectMesh >();
      this._verticesList = new Queue< Vector3 >();
      this._colorsList = new Queue< Vector4 >();
      this._perVerticeData = new Queue< Vector4UInt32 >();
      this._texCoordsList = new Queue< Vector4 >();
      this._normalsList = new Queue< Vector3 >();
      this._ibo = GL.GenBuffer();
    }

    ~VertexBufferObject() {
      this.Dispose( true );
    }

    public void Dispose() {
      this.Dispose( true );
      GC.SuppressFinalize( this );
    }

    protected virtual void Dispose( bool disposing ) {
      if( disposing ) {
        if( this.descriptors != null ) {
          Console.WriteLine( "~VertexBufferObject" );
          GL.DeleteBuffers( VertexBufferObject.DESCRIPTORS_COUNT, this.descriptors );
          GL.DeleteBuffer( this._ibo );
          this.descriptors = null;
          this._meshList = null;
          this._verticesList = null;
          this._colorsList = null;
          this._perVerticeData = null;
          this._texCoordsList = null;
          this._normalsList = null;
          Console.WriteLine( "~VertexBufferObject done" );
        }
      }
    }
  }
}


using System;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace csgl {
  public class Core: GameWindow, IDisposable {
    //static int _q;
    Stopwatch timeRenderInfo;
    Stopwatch timeUpdateInfo;
    protected float delta = 0.0f;
    protected double elapsed = 0.0;
    protected bool isValid = false;
    public ObjectManager objects;
    static public int vao;
    static public int [] vbo;
    static public int tboTexture;
    static public int tbo;

    public unsafe Core()
      : base( 200, 200, GraphicsMode.Default, "Flat engine", GameWindowFlags.FixedWindow, DisplayDevice.Default, 3, 3, GraphicsContextFlags.ForwardCompatible ) {
      this.VSync = VSyncMode.Off;
      this.timeRenderInfo = new Stopwatch();
      this.timeUpdateInfo = new Stopwatch();
      this.isValid = true;
      this.objects = new ObjectManager();

      Console.WriteLine( "load first resource..." );
      //( ( ShaderProgram ) ( ResourceShaderProgram ) Resource.Get( "main.sp" ) ).Use();
      //Resource.Get( "test.tga" );

      GL.Enable( EnableCap.Blend );
      GL.BlendFunc( BlendingFactorSrc.SrcColor, BlendingFactorDest.OneMinusSrcAlpha );

      this.timeRenderInfo.Start();
      this.timeUpdateInfo.Start();

      Texture.TextureType2uniformName.Add( TextureType.TEXTURE0, "texture0" );
      Texture.TextureType2uniformName.Add( TextureType.TEXTURE1, "texture1" );
      Texture.TextureType2uniformName.Add( TextureType.TEXTURE2, "texture2" );
      Texture.TextureType2uniformName.Add( TextureType.TEXTURE3, "texture3" );

      Object obj = this.objects.CreateObject( "obj" );
      obj.position = new Vector2( obj.position.X + 0.1f, obj.position.Y + 0.2f );
      obj.material = ( Material ) Resource.Get( "main.mat" );
      obj.mesh = ( Mesh ) Resource.Get( "sprite.mesh" );

      obj = this.objects.CreateObject( "obj2" );
      obj.position = new Vector2( obj.position.X - 0.2f, obj.position.Y - 0.2f );
      obj.material = ( Material ) Resource.Get( "main-2.mat" );
      obj.mesh = ( Mesh ) Resource.Get( "sprite.mesh" );

      //( ( Material ) Resource.Get( "main-2.mat" ) ).Apply();

      vao = GL.GenVertexArray();
      GL.BindVertexArray( vao );
      vbo = new int[ 3 ];
      GL.GenBuffers( 3, vbo );

      float [] vertPos = {
        -1.0f, -1.0f,
        1.0f, -1.0f,
        -1.0f,  1.0f,
        1.0f,  1.0f,
      };

      float [] vertColor = {
        1.0f, 0.0f, 0.0f, 1.0f,
        0.0f, 1.0f, 0.0f, 1.0f,
        0.0f, 0.0f, 1.0f, 1.0f,
        1.0f, 1.0f, 1.0f, 1.0f,
      };

      UInt32 [] index2matrix = {
        0, 0, 0, 0,
      };

      /* float [] testTbo = new float[ 1024 * 1024 ];
      for( int q = 0; q < testTbo.Length; ++q ) {
        testTbo[ q ] = 0.3f;
      } */

      //modelMatrix
      Matrix4 [] matrixArray = {
        Tools.MakeMatrix( new Vector3( 1.0f, 0.0f, 0.0f ), new Vector3( 0.7f, 0.2f, 1.0f ), ( float ) ( 22.0f * Math.PI / 180.0f ) )
      };

      //worldMatrix
      Parameters.worldMatrix = Tools.MakeMatrix( new Vector3( 0.0f, 0.0f, 0.0f ), new Vector3( 1.0f, 1.0f, 1.0f ), ( float ) ( 0.0f * Math.PI / 180.0f ) );

      Console.WriteLine( GL.GetInteger( GetPName.MaxVertexUniformBlocks ) );

      //0
      /* GL.BindBuffer( BufferTarget.ArrayBuffer, vbo[ 0 ] );
      GL.BufferData( BufferTarget.ArrayBuffer, ( IntPtr ) ( vertPos.Length * sizeof( float ) ), vertPos, BufferUsageHint.StaticDraw );
      GL.VertexAttribPointer( 0, 2, VertexAttribPointerType.Float, false, 0, 0 );
      GL.EnableVertexAttribArray( 0 ); */

      //1
      /* GL.BindBuffer( BufferTarget.ArrayBuffer, vbo[ 1 ] );
      GL.BufferData( BufferTarget.ArrayBuffer, ( IntPtr ) ( vertColor.Length * sizeof( float ) ), vertColor, BufferUsageHint.StaticDraw );
      GL.VertexAttribPointer( 1, 4, VertexAttribPointerType.Float, false, 0, 0 );
      GL.EnableVertexAttribArray( 1 ); */

      //2
      /* GL.BindBuffer( BufferTarget.ArrayBuffer, vbo[ 2 ] );
      GL.BufferData( BufferTarget.ArrayBuffer, ( IntPtr ) ( index2matrix.Length * sizeof( UInt32 ) ), index2matrix, BufferUsageHint.StaticDraw );
      GL.VertexAttribPointer( 2, 1, VertexAttribPointerType.UnsignedInt, false, 0, 0 );
      GL.EnableVertexAttribArray( 2 ); */

      //TBO
      tbo = GL.GenBuffer();
      tboTexture = GL.GenTexture();


      GL.BindBuffer( BufferTarget.TextureBuffer, tbo );
      GL.BufferData( BufferTarget.TextureBuffer, ( IntPtr ) ( sizeof( Matrix4 ) * matrixArray.Length ), matrixArray, BufferUsageHint.StaticDraw );
      GL.BindBuffer( BufferTarget.TextureBuffer, 0 );

      //in Render():
      //GL.ActiveTexture( TextureUnit.Texture0 );
      //GL.BindTexture( TextureTarget.TextureBuffer, tboTexture );
      GL.TexBuffer( TextureBufferTarget.TextureBuffer, SizedInternalFormat.R32f, tbo );
      GL.Uniform1( GL.GetUniformLocation( ( ( ShaderProgram ) Resource.Get( "main-2.sp" ) ).descriptor, "objectsData" ), tboTexture );

      Console.WriteLine( "worldMatrix = {0}", GL.GetUniformLocation( ( ( ShaderProgram ) Resource.Get( "main-2.sp" ) ).descriptor, "worldMatrix" ) );
      Console.WriteLine( "objectsData = {0}", GL.GetUniformLocation( ( ( ShaderProgram ) Resource.Get( "main-2.sp" ) ).descriptor, "objectsData" ) );

      foreach( ShaderProgram shaderProgram in Resource.GetAllShaderProgram() ) {
        shaderProgram.AddUniform( new ShaderProgramUniformMatrix4( "worldMatrix", Parameters.worldMatrix ) );
      }

      Resource.Get( "sprite.mesh" );

      //this.RenderTime = 1000.5;
    }
    //

    ~Core() {
      this.Dispose( true );
    }
    //~

    override public void Dispose() {
      this.Dispose( true );
      GC.SuppressFinalize( this );
    }

    protected override void Dispose( bool disposing ) {
      if( disposing ) {
        if( this.isValid ) {
          Console.WriteLine( "~Core..." );
          this.isValid = false;
          this.objects.Dispose();
          GL.DeleteBuffers( 2, vbo );
          GL.DeleteVertexArray( vao );
          vbo = null;
          vao = 0;
          Resource.DisposeAll();
          Console.WriteLine( "~Core done" );
        }
      }
    }

    protected override void OnUpdateFrame( FrameEventArgs e ) {
      base.OnUpdateFrame( e );

      //timer
      this.delta = ( float ) ( 1000.0 / this.timeUpdateInfo.Elapsed.TotalMilliseconds );
      this.elapsed += this.delta;
      this.timeUpdateInfo.Restart();
      Console.WriteLine( "TPS: {0}", ( int ) this.delta );
    }

    protected override void OnRenderFrame( FrameEventArgs e ) {
      ShaderProgram.UnUse();
      //base.OnRenderFrame( e );

      GL.Viewport( 0, 0, this.Width, this.Height );
      OpenTK.Matrix4 q = new OpenTK.Matrix4();
      q[ 0, 0 ] = 7;
      //GL.Viewport( 0, 0, 100, 100 );
      GL.ClearColor( 1.0f, 1.0f, 1.0f, 1.0f );
      GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

      /* GL.LoadIdentity();
      GL.Begin( PrimitiveType.TriangleStrip );

      float t = ( float ) this.elapsed * 0.0000005f;
      float ct = ( float ) Math.Cos( t ) * 0.2f;
      float st = ( float ) Math.Sin( t ) * 0.2f;
      GL.Vertex3( -1.0f, -1.0f, 0.5f ); //LB
      GL.Vertex3( 1.0f, -1.0f, 0.5f ); //RB
      GL.Vertex3( -1.0f, 1.0f, 0.5f ); //LT
      GL.Vertex3( 1.0f, 1.0f, 0.5f ); //RT

      GL.End(); */

      this.objects.Render();

      SwapBuffers();

      Console.WriteLine( "FPS: {0}", ( int ) ( 1000.0 / this.timeRenderInfo.Elapsed.TotalMilliseconds ) );
      this.timeRenderInfo.Restart();
    }

    protected override void OnKeyDown( OpenTK.Input.KeyboardKeyEventArgs e ) {
      base.OnKeyDown( e );
      if( e.Key == OpenTK.Input.Key.Escape ) {
        this.Close();
      }
    }
  }
}


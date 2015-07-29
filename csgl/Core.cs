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
    //static public int tboTexture;
    //static public int tbo;
    public struct StaticData {
      public ValueFloat t;
      public ValueFloat x;
      public Vector3 position;
      public Vector3 rotation;
      public float FOV;
      public float zNearPlane;
    };

    public StaticData staticData;

    public unsafe Core()
      : base( 200, 200, GraphicsMode.Default, "Flat engine", GameWindowFlags.FixedWindow, DisplayDevice.Default, 3, 3, GraphicsContextFlags.ForwardCompatible ) {
      this.VSync = VSyncMode.Off;
      this.timeRenderInfo = new Stopwatch();
      this.timeUpdateInfo = new Stopwatch();
      this.isValid = true;
      this.objects = new ObjectManager();
      Parameters.tboObjectsMatrices = new TextureBufferObject( "objectsData" );
      Parameters.worldMatrix = new ValueMatrix4();
      Parameters.projectionMatrix = new ValueMatrix4();
      this.staticData.t = new ValueFloat();
      this.staticData.x = new ValueFloat();
      this.staticData.position = new Vector3( 0.0f, 0.0f, -70.0f );
      this.staticData.rotation = new Vector3( 0.0f, 0.0f, 0.0f );
      this.staticData.FOV = 0.02f;
      this.staticData.zNearPlane = 1.0f;

      Console.WriteLine( "load first resource..." );
      //( ( ShaderProgram ) ( ResourceShaderProgram ) Resource.Get( "main.sp" ) ).Use();
      //Resource.Get( "test.tga" );

      GL.Enable( EnableCap.Blend );
      GL.Enable( EnableCap.DepthTest );
      GL.DepthFunc( DepthFunction.Less );
      GL.DepthMask( true );
      GL.FrontFace( FrontFaceDirection.Ccw );
      GL.CullFace( CullFaceMode.FrontAndBack );
      GL.BlendFunc( BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha );

      this.timeRenderInfo.Start();
      this.timeUpdateInfo.Start();

      Texture.TextureType2uniformName.Add( TextureType.TEXTURE0, "texture0" );
      Texture.TextureType2uniformName.Add( TextureType.TEXTURE1, "texture1" );
      Texture.TextureType2uniformName.Add( TextureType.TEXTURE2, "texture2" );
      Texture.TextureType2uniformName.Add( TextureType.TEXTURE3, "texture3" );

      Object obj;

      obj = this.objects.CreateObject( "floor0", ( Model ) Resource.Get( "models/floor.model" ) );
      obj.position = new Vector3( 0.0f, -1.0f, 0.0f );
      obj.scale = new Vector3( 5.0f, 5.0f, 1.0f );
      obj.rotation = new Vector3( Tools.Deg2Rag( 90.0f ), 0.0f, 0.0f );

      obj = this.objects.CreateObject( "obj3", ( Model ) Resource.Get( "models/cube.model" ) );
      obj.position = Vector3.Zero;
      obj.scale = new Vector3( 0.3f, 0.3f, 0.3f );
      obj.rotation = Vector3.Zero;
      //obj.material = ( Material ) Resource.Get( "materials/main-2.mat" );
      //obj.mesh = ( Mesh ) Resource.Get( "meshes/test.mesh" );

      obj = this.objects.CreateObject( "obj", ( Model ) Resource.Get( "models/sprite.model" ) );
      obj.position = new Vector3( 0.5f, 0.0f, 0.0f );
      obj.scale = new Vector3( 0.7f, 0.2f, 1.0f );
      obj.rotation = new Vector3( 0.0f, 0.0f, ( float ) ( 22.0f * Math.PI / 180.0f ) );

      obj = this.objects.CreateObject( "obj2", ( Model ) Resource.Get( "models/sprite.model" ) );
      obj.position = Vector3.Zero;
      obj.scale = new Vector3( 0.7f, 0.2f, 1.0f );
      obj.rotation = new Vector3( 0.0f, 0.0f, ( float ) ( -22.0f * Math.PI / 180.0f ) );
      //obj.material = ( Material ) Resource.Get( "materials/main-2.mat" );
      //obj.mesh = ( Mesh ) Resource.Get( "meshes/test.mesh" );
      //( ( Material ) Resource.Get( "main-2.mat" ) ).Apply();

      this.staticData.t.value = 0;

      vao = GL.GenVertexArray();
      GL.BindVertexArray( vao );
      vbo = new int[ 3 ];
      GL.GenBuffers( 3, vbo );

      Console.WriteLine( Matrix4.CreateFromQuaternion( Quaternion.FromAxisAngle( new Vector3( 1.0f, 1.0f, 1.0f ), 0.5f ) ) );

      /* float [] vertPos = {
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
      }; */

      /* float [] testTbo = new float[ 1024 * 1024 ];
      for( int q = 0; q < testTbo.Length; ++q ) {
        testTbo[ q ] = 0.3f;
      } */

      //modelMatrix
      /* Matrix4 [] matrixArray = {
        Tools.MakeMatrix( new Vector3( 0.5f, 0.0f, 0.0f ), new Vector3( 0.7f, 0.2f, 1.0f ), ( float ) ( 22.0f * Math.PI / 180.0f ) ),
        Tools.MakeMatrix( new Vector3( 0.0f, 0.0f, 0.0f ), new Vector3( 0.7f, 0.2f, 1.0f ), ( float ) ( -22.0f * Math.PI / 180.0f ) ),
        Tools.MakeMatrix( new Vector3( 0.0f, 0.0f, 0.0f ), new Vector3( 0.3f, 0.3f, 1.0f ), 0.0f ),
      };
      UInt32 index = 0;
      foreach( Matrix4 matrix in matrixArray ) {
        Parameters.tboObjectsMatrices.SetData( matrix, index++ );
      } */

      //worldMatrix
      Parameters.worldMatrix.value = Tools.MakeMatrix( new Vector3( 0.0f, 0.0f, 0.0f ), new Vector3( 1.0f, 1.0f, 1.0f ), ( float ) ( 0.0f * Math.PI / 180.0f ) );

      Parameters.projectionMatrix.value = Tools.MakeMatrixPerspective(
        new Vector3( 0.0f, -1.0f, 0.0f ),
        new Vector3( 0.0f, 0.0f, 0.0f ),
        0.01f,
        1000.0f,
        ( float ) Math.PI * 10.1f,
        1.0f
      );

      //Console.WriteLine( GL.GetInteger( GetPName.MaxVertexUniformBlocks ) );

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
      //tbo = GL.GenBuffer();
      //tboTexture = GL.GenTexture();

      //GL.BindBuffer( BufferTarget.TextureBuffer, tbo );
      //GL.BufferData( BufferTarget.TextureBuffer, ( IntPtr ) ( sizeof( Matrix4 ) * matrixArray.Length ), matrixArray, BufferUsageHint.StaticDraw );
      //GL.BindBuffer( BufferTarget.TextureBuffer, 0 );
      /* fixed( void* pMatrixArray = matrixArray ) {
        //Parameters.tboObjectsMatrices.SetData( ( IntPtr ) pMatrixArray );
      } */

      //in Render():
      //GL.ActiveTexture( TextureUnit.Texture0 );
      //GL.BindTexture( TextureTarget.TextureBuffer, tboTexture );
      //GL.TexBuffer( TextureBufferTarget.TextureBuffer, SizedInternalFormat.R32f, tbo );
      //GL.Uniform1( GL.GetUniformLocation( ( ( ShaderProgram ) Resource.Get( "main-2.sp" ) ).descriptor, "objectsData" ), tboTexture );

      foreach( ShaderProgram shaderProgram in Resource.GetAllShaderProgram() ) {
        shaderProgram.AddUniform( new ShaderProgramUniformMatrix4Pointer( "worldMatrix", Parameters.worldMatrix ) );
        shaderProgram.AddUniform( new ShaderProgramUniformMatrix4Pointer( "projectionMatrix", Parameters.projectionMatrix ) );
        shaderProgram.AddUniform( new ShaderProgramUniformFloatPointer( "time", this.staticData.t ) );
        Parameters.tboObjectsMatrices.Bind( shaderProgram );
      }

      GL.Viewport( 0, 0, this.Width, this.Height );
      //Resource.Get( "sprite.mesh" );

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
          Parameters.tboObjectsMatrices.Dispose();
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
      this.delta = ( float ) e.Time;
      this.elapsed += this.delta;
      this.timeUpdateInfo.Restart();
      this.objects.Update();
      unsafe {
        Console.WriteLine( "TPS: {0}", ( int ) ( this.delta * 1000000.0f ) );
      }

      //Parameters.worldMatrix.value = Tools.MakeMatrix( new Vector3( 0.0f, ( float ) Math.Sin( this.staticData.t.value * 10.0f ), 0.0f ), new Vector3( 1.0f, 1.0f, 1.0f ), ( float ) ( 0.0f * Math.PI / 180.0f ) );
      float s = ( float ) Math.Sin( this.staticData.t.value * 2.0f ) + 1.0f;
      float s2 = ( float ) Math.Sin( this.staticData.t.value * 4.0f );
      Parameters.worldMatrix.value = Tools.MakeMatrix(
        this.staticData.position,
        Vector3.One,
        this.staticData.rotation
      );
      Parameters.projectionMatrix.value = Tools.MakeMatrixProjectionPerspective(
        this.staticData.zNearPlane,
        1000.0f,
        this.staticData.FOV, //380
        1.0f
      );
      //Parameters.projectionMatrix.value.Transpose();

      this.staticData.t.value += ( float ) e.Time;
      Object obj = this.objects.Get( "obj3" );
      obj.position = new Vector3( ( float ) Math.Sin( this.staticData.t * 2.0f ) * 0.5f, 0.0f, 0.0f );
      obj.rotation = new Vector3( 0.0f, 0.0f, ( float ) ( Math.Sin( this.staticData.t.value * 0.2f ) * Math.PI * 2.0f ) );
    }

    protected override void OnRenderFrame( FrameEventArgs e ) {
      ShaderProgram.UnUse();
      //base.OnRenderFrame( e );

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
      /* if( e.Key == OpenTK.Input.Key.Q ) {
        this.staticData.x.value -= 0.1f;
      }
      if( e.Key == OpenTK.Input.Key.W ) {
        this.staticData.x.value += 0.1f;
      } */
      if( e.Key == OpenTK.Input.Key.W ) {
        this.staticData.position.Y -= 0.1f;
      }
      if( e.Key == OpenTK.Input.Key.S ) {
        this.staticData.position.Y += 0.1f;
      }
      if( e.Key == OpenTK.Input.Key.A ) {
        this.staticData.position.X += 0.1f;
      }
      if( e.Key == OpenTK.Input.Key.D ) {
        this.staticData.position.X -= 0.1f;
      }
      if( e.Key == OpenTK.Input.Key.Q ) {
        this.staticData.position.Z += 1.0f;
      }
      if( e.Key == OpenTK.Input.Key.E ) {
        this.staticData.position.Z -= 1.0f;
      }
      if( e.Key == OpenTK.Input.Key.Number1 ) {
        this.staticData.rotation.X -= 0.1f;
      }
      if( e.Key == OpenTK.Input.Key.Number2 ) {
        this.staticData.rotation.X += 0.1f;
      }
      if( e.Key == OpenTK.Input.Key.Number3 ) {
        this.staticData.rotation.Y -= 0.1f;
      }
      if( e.Key == OpenTK.Input.Key.Number4 ) {
        this.staticData.rotation.Y += 0.1f;
      }
      if( e.Key == OpenTK.Input.Key.Number5 ) {
        this.staticData.rotation.Z += 0.1f;
      }
      if( e.Key == OpenTK.Input.Key.Number6 ) {
        this.staticData.rotation.Z -= 0.1f;
      }
      if( e.Key == OpenTK.Input.Key.Z ) {
        this.staticData.FOV += 0.02f;
      }
      if( e.Key == OpenTK.Input.Key.X ) {
        this.staticData.FOV -= 0.02f;
      }
      if( e.Key == OpenTK.Input.Key.C ) {
        this.staticData.zNearPlane += 0.1f;
      }
      if( e.Key == OpenTK.Input.Key.V ) {
        this.staticData.zNearPlane -= 0.1f;
      }
    }
  }
}


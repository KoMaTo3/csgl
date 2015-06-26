using System;
using Gdk;

namespace csgl {
  public class ImageLoader {
    public ImageLoader() {
    }

    public static unsafe Image Load( byte [] buffer ) {
      int length = buffer.Length;

      if( buffer == null || length < 1 ) {
        return null;
      }

      fixed( byte *data = buffer ) {
        if( length >= 2 ) { //BMP
          if(
            data[ 0 ] == 'B' &&
            data[ 1 ] == 'M' ) {
            //return ImageLoader.LoadFromBMP( buffer );
          }
        }
        if( length >= 3 ) { //TGA
          if(
            data[ 0 ] == 0 &&
            data[ 1 ] == 0 &&
            ( data[ 2 ] == 0x02 || data[ 2 ] == 0x0A ) ) {
            return ImageLoader.LoadFromTGA( buffer );
          }
        }
        if( length >= 4 ) { //PNG
          if(
            data[ 0 ] == 0x89 &&
            data[ 1 ] == 'P' &&
            data[ 2 ] == 'N' &&
            data[ 3 ] == 'G' ) {
            //return ImageLoader.LoadFromPNG( buffer );
          }
        }
        if( length >= 10 ) { //JPG
          if(
            data[ 6 ] == 'J' &&
            data[ 7 ] == 'F' &&
            data[ 8 ] == 'I' &&
            data[ 9 ] == 'F' ) {
            //return ImageLoader.LoadFromJPG( buffer );
          }
        }
      }

      return null;
    }

    public static unsafe Image LoadFromTGA( byte [] buffer ) {
      Image result = null;
      int length = buffer.Length;

      if( buffer == null || length < 1 ) {
        return result;
      }

      byte idLength = buffer[ 0 ];
      if( buffer[ 1 ] != 0 ) {
        return result;
      }

      byte compressed = buffer[ 2 ]; //RLE-compression
      if( !( compressed == 2 || compressed == 0x0A ) ) {
        return result;
      }

      fixed( byte *tmp = buffer ) {
        UInt16* w = ( UInt16* ) ( tmp + 12 );
        UInt16* h = ( UInt16* ) ( tmp + 14 );

        byte bpp = tmp[ 16 ];
        if( !( bpp == 24 || bpp == 32 ) ) {
          return result;
        }

        UInt32 src_pos = 18 + ( UInt32 ) idLength;
        Size size = new Size();

        size.Width = *w;
        size.Height = *h;
        byte [] _data = new byte[ size.Width * size.Height * 4 ];
        fixed( byte *dest = _data ) {
          if( compressed == 2 ) { //not compressed
            UInt16 x, y;
            byte mult = ( byte ) ( bpp >> 3 );
            for( y = 0; y < size.Height; y++ )
              for( x = 0; x < size.Width; x++ ) {
                UInt32 dest_pos = ( UInt32 ) ( x + y * size.Width ) << 2;
                //dest_pos = ( x + ( size.Height - y - 1 ) * size.Width ) << 2;
                src_pos = ( UInt32 ) ( x + y * size.Width ) * mult + 18;
                dest[ dest_pos ] = tmp[ src_pos + 2 ];
                dest[ dest_pos + 1 ] = tmp[ src_pos + 1 ];
                dest[ dest_pos + 2 ] = tmp[ src_pos + 0 ];
                dest[ dest_pos + 3 ] = ( byte ) ( bpp == 32 ? tmp[ src_pos + 3 ] : 255 );
              }
          } else { //RLE-compression 0x0A
            src_pos = ( UInt32 ) ( 18 + idLength );
            UInt32 dest_pos = 0;
            byte q;
            byte r, g, b, a = 255;
            UInt32 x = 0, y = 0;
            while( y < size.Height ) {
              byte block = tmp[ src_pos++ ];
              byte num = ( byte ) ( block & 127 );

              if( ( block & 128 ) == 128 ) { //compressed block
                b = tmp[ src_pos ];
                g = tmp[ src_pos + 1 ];
                r = tmp[ src_pos + 2 ];
                if( bpp == 32 ) {
                  a = tmp[ src_pos + 3 ];
                  src_pos += 4;
                } else
                  src_pos += 3;
                for( q = 0; q < num + 1; ++q ) {
                  //dest_pos = ( x + y * size.Width ) << 2;
                  dest_pos = ( UInt32 ) ( x + ( size.Height - y - 1 ) * size.Width ) << 2;
                  dest[ dest_pos ] = r;
                  dest[ dest_pos + 1 ] = g;
                  dest[ dest_pos + 2 ] = b;
                  dest[ dest_pos + 3 ] = a;

                  x = ( UInt32 ) ( ( x + 1 ) % size.Width );
                  if( x == 0 ) {
                    ++y;
                  }
                }
              } else { //not compressed block
                for( q = 0; q < num + 1; ++q ) {
                  //dest_pos = ( x + y * size.Width ) << 2;
                  dest_pos = ( UInt32 ) ( x + ( size.Height - y - 1 ) * size.Width ) << 2;
                  b = tmp[ src_pos ];
                  g = tmp[ src_pos + 1 ];
                  r = tmp[ src_pos + 2 ];
                  if( bpp == 32 ) {
                    a = tmp[ src_pos + 3 ];
                    src_pos += 4;
                  } else
                    src_pos += 3;

                  dest[ dest_pos ] = r;
                  dest[ dest_pos + 1 ] = g;
                  dest[ dest_pos + 2 ] = b;
                  dest[ dest_pos + 3 ] = a;

                  x = ( UInt32 ) ( ( x + 1 ) % size.Width );
                  if( x == 0 ) {
                    ++y;
                  }
                }//for q < num
              }//not compressed block
            }//while y < height
          }//RLE-compression
        }

        result = new Image( _data, size.Width, size.Height );
      }

      return result;
    }
  }
}


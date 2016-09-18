#include "texture.hpp"
#include <string>

GLuint textureHandles[NUM_TEXTURES + 1];

void createAndTexture(GLuint handle)
{
  GLubyte data[256][256][2];
  for (int x = 0; x < 256; x++)
    for (int y = 0; y < 256; y++)
    {
      data[x][y][0] = x & y;
      data[x][y][1] = 255;
    }

  glBindTexture(GL_TEXTURE_2D, handle);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
  gluBuild2DMipmaps(GL_TEXTURE_2D, GL_LUMINANCE_ALPHA, 256, 256, GL_LUMINANCE_ALPHA, GL_UNSIGNED_BYTE, data);
  glTexEnvf(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_MODULATE);
}

void createXorTexture(GLuint handle)
{
  GLubyte data[256][256][2];
  for (int x = 0; x < 256; x++)
    for (int y = 0; y < 256; y++)
    {
      data[x][y][0] = x ^ y;
      data[x][y][1] = 255;
    }

  glBindTexture(GL_TEXTURE_2D, handle);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
  gluBuild2DMipmaps(GL_TEXTURE_2D, GL_LUMINANCE_ALPHA, 256, 256, GL_LUMINANCE_ALPHA, GL_UNSIGNED_BYTE, data);
  glTexEnvf(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_MODULATE);
}

void createOrTexture(GLuint handle)
{
  GLubyte data[256][256][2];
  for (int x = 0; x < 256; x++)
    for (int y = 0; y < 256; y++)
    {
      data[x][y][0] = x | y;
      data[x][y][1] = 255;
    }

  glBindTexture(GL_TEXTURE_2D, handle);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
  gluBuild2DMipmaps(GL_TEXTURE_2D, GL_LUMINANCE_ALPHA, 256, 256, GL_LUMINANCE_ALPHA, GL_UNSIGNED_BYTE, data);
  glTexEnvf(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_MODULATE);
}

void createMirrorTexture(GLuint handle)
{
  GLubyte data[256][256][2];
  for (int x = 0; x < 256; x++)
    for (int y = 0; y < 256; y++)
    {
      data[x][y][0] = (GLubyte)(160 + 95 * sin(2 * M_PI * y / 256.0 + 2 * M_PI * x / 256.0));
      data[x][y][1] = 255;
    }

  glBindTexture(GL_TEXTURE_2D, handle);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
  gluBuild2DMipmaps(GL_TEXTURE_2D, GL_LUMINANCE_ALPHA, 256, 256, GL_LUMINANCE_ALPHA, GL_UNSIGNED_BYTE, data);
  glTexEnvf(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_MODULATE);
}

void createSandTexture(GLuint handle)
{
  GLubyte data[256][256][2];
  for (int x = 0; x < 256; x++)
    for (int y = 0; y < 256; y++)
    {
      data[x][y][0] = (GLubyte)(160 + 95 * sin(8 * M_PI * y / 256.0 + 0.25 * M_PI * cos(2 * M_PI * x / 256.0)));
      data[x][y][1] = 255;
    }

  glBindTexture(GL_TEXTURE_2D, handle);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
  gluBuild2DMipmaps(GL_TEXTURE_2D, GL_LUMINANCE_ALPHA, 256, 256, GL_LUMINANCE_ALPHA, GL_UNSIGNED_BYTE, data);
  glTexEnvf(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_MODULATE);
}

void createRockTexture(GLuint handle)
{
  GLubyte data[256][256][2];
  for (int x = 0; x < 256; x++)
    for (int y = 0; y < 256; y++)
    {
      data[x][y][0] = (GLubyte)(127 + 16 * sin(2 * M_PI * x / 256.0) + 16 * sin(2 * M_PI * y / 256.0) +
                      16 * sin(4 * M_PI * ((x+37)%256) / 256.0 + 2 * M_PI * y / 256.0) + 16 * sin(4 * M_PI * ((y+57)%256) / 256.0+ 5 * M_PI * x / 256.0) +
                      16 * sin(8 * M_PI * ((x+119)%256) / 256.0 + 3 * M_PI * y / 256.0) + 16 * sin(8 * M_PI * ((y+27)%256) / 256.0+ 2 * M_PI * x / 256.0) + 
                      16 * sin(16 * M_PI * ((x+32)%256) / 256.0 + 7 * M_PI * y / 256.0) + 16 * sin(16 * M_PI * ((y+64)%256) / 256.0 + 9 * M_PI * x / 256.0));
      data[x][y][1] = 255;
    }

  glBindTexture(GL_TEXTURE_2D, handle);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
  gluBuild2DMipmaps(GL_TEXTURE_2D, GL_LUMINANCE_ALPHA, 256, 256, GL_LUMINANCE_ALPHA, GL_UNSIGNED_BYTE, data);
  glTexEnvf(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_MODULATE);
}

void createWaterTexture(GLuint handle)
{
  GLubyte data[256][256][2];
  for (int x = 0; x < 256; x++)
    for (int y = 0; y < 256; y++)
    {
      data[x][y][0] = (GLubyte)(191 + 32 * sin(8 * M_PI * y / 256.0 + 0.5 * M_PI * cos(2 * M_PI * x / 256.0)) +
                          + 32 * sin(8 * M_PI * x / 256.0 + 0.25 * M_PI * cos(2 * M_PI * y / 256.0)));
      data[x][y][1] = 255;
    }

  glBindTexture(GL_TEXTURE_2D, handle);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
  gluBuild2DMipmaps(GL_TEXTURE_2D, GL_LUMINANCE_ALPHA, 256, 256, GL_LUMINANCE_ALPHA, GL_UNSIGNED_BYTE, data);
  glTexEnvf(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_MODULATE);
}

void createPanelTexture(GLuint handle)
{
  GLubyte data[256][256][2];
  for (int x = 0; x < 256; x++)
    for (int y = 0; y < 256; y++)
    {
      data[x][y][0] = 64 + (unsigned char)(128 * ((x & y) / 255.0));
      if (x < 16 || y < 16) data[x][y][0] = std::min(x, y) * 8;
      if ((x > 239 || y > 239) && x + y > 255) data[x][y][0] = 127 + (std::max(x, y) - 240) * 8;
      data[x][y][1] = 255;
    }

  glBindTexture(GL_TEXTURE_2D, handle);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
  gluBuild2DMipmaps(GL_TEXTURE_2D, GL_LUMINANCE_ALPHA, 256, 256, GL_LUMINANCE_ALPHA, GL_UNSIGNED_BYTE, data);
  glTexEnvf(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_MODULATE);
}

void createMarioTexture(GLuint handle)
{
  GLubyte data[256][256][2];
  for (int x = 0; x < 256; x++)
    for (int y = 0; y < 256; y++)
    {
      if ((x%128) + (y%128) > 127) data[x][y][0] = 8 * std::min(127 - (x%128), 127 - (y%128));
      else data[x][y][0] = 8 * std::min(x%128, y%128);
      data[x][y][1] = 255;
    }

  glBindTexture(GL_TEXTURE_2D, handle);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
  gluBuild2DMipmaps(GL_TEXTURE_2D, GL_LUMINANCE_ALPHA, 256, 256, GL_LUMINANCE_ALPHA, GL_UNSIGNED_BYTE, data);
  glTexEnvf(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_MODULATE);
}

void createBHoleTexture(GLuint handle)
{
  GLubyte data[256][256][2];
  for (int x = 0; x < 256; x++)
    for (int y = 0; y < 256; y++)
    {
      data[x][y][0] = 255;
      double dist = (Vector2D(x, y) - Vector2D(128, 128)).length();
      if (dist > 128) data[x][y][1] = 0;
      else if (dist < 64) data[x][y][1] = 255;
      else data[x][y][1] = (unsigned char)(255 * (1 - ((dist-64) / 64)));
    }

  glBindTexture(GL_TEXTURE_2D, handle);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
  gluBuild2DMipmaps(GL_TEXTURE_2D, GL_LUMINANCE_ALPHA, 256, 256, GL_LUMINANCE_ALPHA, GL_UNSIGNED_BYTE, data);
  glTexEnvf(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_MODULATE);
}

void createPipeHTexture(GLuint handle)
{
  GLubyte data[256][256][2];
  for (int x = 0; x < 256; x++)
    for (int y = 0; y < 256; y++)
    {
      data[x][y][0] = 64 + (GLubyte)(191 * abs((double)sin(2 * M_PI * x / 64)));
      data[x][y][1] = 255;
    }

  glBindTexture(GL_TEXTURE_2D, handle);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
  gluBuild2DMipmaps(GL_TEXTURE_2D, GL_LUMINANCE_ALPHA, 256, 256, GL_LUMINANCE_ALPHA, GL_UNSIGNED_BYTE, data);
  glTexEnvf(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_MODULATE);
}

void createPipeVTexture(GLuint handle)
{
  GLubyte data[256][256][2];
  for (int x = 0; x < 256; x++)
    for (int y = 0; y < 256; y++)
    {
      data[x][y][0] = (GLubyte)(64 + 191 * abs((double)sin(2 * M_PI * y / 64)));
      data[x][y][1] = 255;
    }

  glBindTexture(GL_TEXTURE_2D, handle);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
  gluBuild2DMipmaps(GL_TEXTURE_2D, GL_LUMINANCE_ALPHA, 256, 256, GL_LUMINANCE_ALPHA, GL_UNSIGNED_BYTE, data);
  glTexEnvf(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_MODULATE);
}

void createPillarTexture(GLuint handle)
{
  GLubyte data[256][256][2];
  for (int x = 0; x < 256; x++)
    for (int y = 0; y < 256; y++)
    {
      data[x][y][0] = 128 + (unsigned char)(127 * ((x | y) / 255.0));
      if (y % 128 < 4) data[x][y][0] = 128;
      if (x < 4) data[x][y][0] = 128;
      if (x == 192 || x == 193) data[x][y][0] = 128;
      if (y % 128 > 123) data[x][y][0] = 255;
      if (x > 251) data[x][y][0] = 255;
      if (x == 191 || x == 190) data[x][y][0] = 255;
      
      data[x][y][1] = 255;
    }

  glBindTexture(GL_TEXTURE_2D, handle);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
  gluBuild2DMipmaps(GL_TEXTURE_2D, GL_LUMINANCE_ALPHA, 256, 256, GL_LUMINANCE_ALPHA, GL_UNSIGNED_BYTE, data);
  glTexEnvf(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_MODULATE);
}

void createCarpetTexture(GLuint handle)
{
  GLubyte data[256][256][2];
  for (int x = 0; x < 256; x++)
    for (int y = 0; y < 256; y++)
    {
      data[x][y][0] = (unsigned char)(255 * (0.2 + 0.4*((x%128) + (x%64) + (x%32) + (x%16))
                                 + 0.4*((y%128) + (y%64) + (y%32) + (y%16))));
      
      data[x][y][1] = 255;
    }

  glBindTexture(GL_TEXTURE_2D, handle);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
  gluBuild2DMipmaps(GL_TEXTURE_2D, GL_LUMINANCE_ALPHA, 256, 256, GL_LUMINANCE_ALPHA, GL_UNSIGNED_BYTE, data);
  glTexEnvf(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_MODULATE);
}

void createFPanelTexture(GLuint handle)
{
  GLubyte data[256][256][2];
  for (int x = 0; x < 256; x++)
    for (int y = 0; y < 256; y++)
    {
      data[x][y][0] = (GLubyte)(128 + 127 * std::min(abs((double)sin(2 * M_PI * x / 64) + sin(2 * M_PI * y / 64)), 1.0));
      data[x][y][1] = 255;
    }

  glBindTexture(GL_TEXTURE_2D, handle);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
  gluBuild2DMipmaps(GL_TEXTURE_2D, GL_LUMINANCE_ALPHA, 256, 256, GL_LUMINANCE_ALPHA, GL_UNSIGNED_BYTE, data);
  glTexEnvf(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_MODULATE);
}

void createTextures()
{
  textureHandles[0] = 0;
  glGenTextures(NUM_TEXTURES, &textureHandles[1]);

  createMarioTexture(textureHandles[MARIO_TEXTURE]);
  createMirrorTexture(textureHandles[MIRROR_TEXTURE]);
  createPanelTexture(textureHandles[PANEL_TEXTURE]);
  createRockTexture(textureHandles[ROCK_TEXTURE]);
  createSandTexture(textureHandles[SAND_TEXTURE]);
  createWaterTexture(textureHandles[WATER_TEXTURE]);
  createAndTexture(textureHandles[AND_TEXTURE]);
  createOrTexture(textureHandles[OR_TEXTURE]);
  createXorTexture(textureHandles[XOR_TEXTURE]);
  createBHoleTexture(textureHandles[BHOLE_TEXTURE]);
  createPipeVTexture(textureHandles[PIPEV_TEXTURE]);
  createPipeHTexture(textureHandles[PIPEH_TEXTURE]);
  createPillarTexture(textureHandles[PILLAR_TEXTURE]);
  createCarpetTexture(textureHandles[CARPET_TEXTURE]);
  createFPanelTexture(textureHandles[FPANEL_TEXTURE]);
}

WallTexture::WallTexture():color(0,0,0), textureRef(0)
{
}

void WallTexture::operator >>(std::istream& in)
{
  std::string devNull;
  in >> devNull; // {
  double r, g, b;
  unsigned int ref;
  in >> r;
  in >> g;
  in >> b;
  in >> ref;
  color = Colour(r, g, b);
  textureRef = ref;
  in >> devNull; // }
}

void WallTexture::operator=(const WallTexture& other)
{
  color = other.color;
  textureRef = other.textureRef;
}

void WallTexture::apply()
{
  glColor3f((GLfloat)color.R(), (GLfloat)color.G(), (GLfloat)color.B());
  glBindTexture(GL_TEXTURE_2D, textureHandles[textureRef]);
}

bool WallTexture::isDefined()
{
  return textureRef != 0;
}

Colour WallTexture::getColour()
{
  return color;
}

WallTexture WallTexture::getMirrorTexture()
{
  WallTexture result;
  result.color = Colour(0.5f, 0.5f, 1.0f);
  result.textureRef = ROCK_TEXTURE;
  return result;
}

WallTexture WallTexture::getPortalTexture()
{
  WallTexture result;
  result.color = Colour(1.0f, 0.0f, 1.0f);
  result.textureRef = WATER_TEXTURE;
  return result;
}

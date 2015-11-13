#ifndef _TEXTURE_
#define _TEXTURE_

#include "SDL_opengl.h"
#include "algebra.hpp"
#include <istream>

#define NUM_TEXTURES 16
#define NO_TEXTURE 0
#define MARIO_TEXTURE 1
#define PANEL_TEXTURE 2
#define MIRROR_TEXTURE 3
#define ROCK_TEXTURE 4
#define SAND_TEXTURE 5
#define WATER_TEXTURE 6
#define AND_TEXTURE 7
#define OR_TEXTURE 8
#define XOR_TEXTURE 9
#define BHOLE_TEXTURE 10
#define PIPEV_TEXTURE 11
#define PIPEH_TEXTURE 12
#define PILLAR_TEXTURE 13
#define CARPET_TEXTURE 14
#define FPANEL_TEXTURE 15
extern GLuint textureHandles[NUM_TEXTURES + 1];
void createTextures();

class WallTexture
{
public:
  WallTexture();
  void operator=(const WallTexture& other);
  void operator>>(std::istream& in);
  void apply();
  bool isDefined();
  Colour getColour();
  static WallTexture getMirrorTexture();
  static WallTexture getPortalTexture();
private:
  Colour color;
  unsigned int textureRef;
};

struct TextureSet {
  WallTexture upper;
  WallTexture middle;
  WallTexture lower;
};

#endif

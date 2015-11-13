#include "map.hpp"
#include "sector.hpp"
#include "misc.hpp"
#include "SDL_opengl.h"
#include "texture.hpp"
#include <fstream>
#include <string>

Map::Map(char* filename)
{
  std::string devNull;
  std::ifstream in(filename);
  if (!in)
  {
    std::cerr << "Error opening file " << filename << std::endl;
    std::exit(-1);
  }

  // Initialize vertex list

  unsigned int vertexCount;
  in >> devNull; //VCOUNT
  in >> vertexCount;
  std::vector<Point2D> vertices(vertexCount);

  for (unsigned int vert = 0; vert < vertexCount; vert++)
  {
    double x, y;
    in >> devNull; //V
    in >> x;
    in >> y;
    vertices[vert] = Point2D(x, y);
  }

  // Initialize sector/wall list

  unsigned int sectorCount;
  in >> devNull; //SCOUNT
  in >> sectorCount;
  sectors.resize(sectorCount);

  for (unsigned int sect = 0; sect < sectorCount; sect++)
  {
    unsigned int wallCount;
    int floorHeight, ceilingHeight;
    in >> devNull; //S
    in >> devNull; //F
    WallTexture floorTex;
    floorTex >> in;
    in >> floorHeight;
    in >> devNull; //C
    WallTexture ceilingTex;
    ceilingTex >> in;
    in >> ceilingHeight;
    in >> devNull; //WCOUNT
    in >> wallCount;
    std::vector<Wall*> sectorWalls(wallCount);
    std::vector<TextureSet> wallTextures(wallCount);
    std::vector<unsigned int> wallVertices(wallCount);
    std::vector<std::string> wallFlags(wallCount);
    std::vector<char> wallRefs(wallCount);

    for (unsigned int wall = 0; wall < wallCount; wall++)
    {
      in >> devNull; //W
      wallTextures[wall].lower >> in;
      wallTextures[wall].middle >> in;
      wallTextures[wall].upper >> in;
      in >> wallVertices[wall];
      in >> wallFlags[wall];
      in >> wallRefs[wall];
    }
    for (unsigned int wall = 0; wall < wallCount; wall++)
    {
      Wall* newWall = new Wall(vertices[wallVertices[wall]],
        vertices[wallVertices[(wall+1) % wallCount]],
        wallTextures[wall],
        wallFlags[wall],
        wallRefs[wall]);
      walls.push_back(newWall);
      sectorWalls[wall] = newWall;
    }
    Sector* newSect = new Sector(sectorWalls, floorTex, floorHeight, ceilingTex, ceilingHeight);
    sectors[sect] = newSect;
  }

  double spawnX, spawnY;
  in >> spawnX;
  in >> spawnY;

  playerSpawnPoint = Point2D(spawnX, spawnY);
  in >> playerSpawnSector;
  in >> playerSpawnAngle;

  in >> devNull; // ECOUNT
  unsigned int numEnemies;
  in >> numEnemies;

  enemySpawnPoints.resize(numEnemies);
  enemySpawnSectors.resize(numEnemies);
  enemySpawnAngles.resize(numEnemies);

  for (unsigned int count = 0; count < numEnemies; count++)
  {
    in >> spawnX;
    in >> spawnY;
    enemySpawnPoints[count] = Point2D(spawnX, spawnY);
    in >>enemySpawnSectors[count];
    in >> enemySpawnAngles[count];
  }

  in.close();

  std::cout << std::endl;
  for (std::vector<Wall*>::iterator first = walls.begin(); first < walls.end(); first++)
  {
    for (std::vector<Wall*>::iterator second = first; second < walls.end(); second++)
    {
      (*first)->link(*second);
    }
  }
}

Map::~Map()
{
  for (std::vector<Wall*>::iterator iter  = walls.begin(); iter < walls.end(); iter++) delete *iter;
  for (std::vector<Sector*>::iterator iter  = sectors.begin(); iter < sectors.end(); iter++) delete *iter;
  for (std::vector<Entity*>::iterator iter  = entities.begin(); iter < entities.end(); iter++) delete *iter;
}

Player* Map::spawnEntities()
{
  Player* result =  new Player(playerSpawnPoint, playerSpawnAngle, sectors[playerSpawnSector]);
  entities.push_back(result);

  for (unsigned int count = 0; count < enemySpawnPoints.size(); count++)
  {
    Enemy* newEnemy = new Enemy(enemySpawnPoints[count], enemySpawnAngles[count], sectors[enemySpawnSectors[count]], result);
    entities.push_back(newEnemy);
  }
  return result;
}

std::ostream& Map::operator<<(std::ostream& out)
{
  out << "Sectors: " << sectors.size() << std::endl;
  for (unsigned int count = 0; count < sectors.size(); count++)
  {
    (*sectors[count]) << out;
  }
  return out;
}

void Map::update(double time)
{
  std::vector<ProjectileBase*> projectiles;
  for (std::vector<Sector*>::iterator iter = sectors.begin(); iter != sectors.end(); iter++) (*iter)->getProjectiles(projectiles);
  for (std::vector<ProjectileBase*>::iterator iter = projectiles.begin(); iter != projectiles.end(); iter++) (*iter)->update(time);
  for (std::vector<Sector*>::iterator iter = sectors.begin(); iter != sectors.end(); iter++) (*iter)->update(time);
  for (std::vector<Entity*>::iterator iter  = entities.begin(); iter < entities.end(); iter++) (*iter)->update(time);
}

void Map::render2D(Point2D pos, double angle)
{
  glMatrixMode(GL_PROJECTION);
  glLoadIdentity();
  GLint viewport[4];
  glGetIntegerv(GL_VIEWPORT, viewport);
  double ar = (double)viewport[2] / (double)viewport[3];
  glOrtho(-MIN_2D_SCALE * ar, MIN_2D_SCALE * ar, -MIN_2D_SCALE, MIN_2D_SCALE, 0, 10000);
  glMatrixMode(GL_MODELVIEW);
  glLoadIdentity();
  glRotated(-angle, 0,0,1);
  glTranslated(-pos[0], -pos[1], 0);

  glBindTexture(GL_TEXTURE_2D, textureHandles[NO_TEXTURE]);
  glDisable(GL_LIGHTING);
  for (std::vector<Sector*>::iterator iter = sectors.begin(); iter != sectors.end(); iter++) (*iter)->render2D();
  glEnable(GL_LIGHTING);
}
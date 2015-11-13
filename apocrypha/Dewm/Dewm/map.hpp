#ifndef __MAP__
#define __MAP__

#include "wall.hpp"
#include <vector>
#include "player.hpp"

class Sector;

class Map{
public:
  Map(char* filename);
  virtual ~Map();
  Player* spawnEntities();
  void update(double time);
  void render2D(Point2D pos, double angle);

  std::ostream& operator<<(std::ostream& out);

private:
  std::vector<Sector*> sectors;
  std::vector<Wall*> walls;
  std::vector<Entity*> entities;
  Point2D playerSpawnPoint;
  unsigned int playerSpawnSector;
  double playerSpawnAngle;
  std::vector<Point2D> enemySpawnPoints;
  std::vector<unsigned int> enemySpawnSectors;
  std::vector<double> enemySpawnAngles;
};

#endif

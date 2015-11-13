#include "sector.hpp"
#include "line.hpp"
#include "SDL_opengl.h"
#include "misc.hpp"
#include "dlist.hpp"
#include "player.hpp"

Sector::Sector(std::vector<Wall*>& walls,
               WallTexture floorTex,
               int floorHeight,
               WallTexture ceilingTex,
               int ceilingHeight)
:F(floorHeight), C(ceilingHeight), floor(floorTex), ceiling(ceilingTex), W(walls)
{
  for (std::vector<Wall*>::iterator iter = W.begin(); iter < W.end(); iter++)
  {
    (*iter)->setFrontSector(this);
  }
}

void Sector::render2D()
{
  for (std::vector<Wall*>::iterator iter = W.begin(); iter < W.end(); iter++)
  {
    (*iter)->render2D();
  }

  for (std::set<Entity*>::iterator iter = entities.begin(); iter != entities.end(); iter++)
  {
    (*iter)->render2D();
  }
}

void Sector::render3D(Point2D pos, Point2D left, Point2D right, unsigned int bouncesLeft)
{
  GLfloat pos0[4] = {1000.0f, 1000.0f, 1000.0f, 1.0f};
  glLightfv(GL_LIGHT0, GL_POSITION, pos0);

  for (std::vector<Wall*>::iterator iter = W.begin(); iter < W.end(); iter++)
  {
    (*iter)->render3D(pos, left, right, bouncesLeft);
  }

  std::vector<Point2D> vertices;
  for (std::vector<Wall*>::iterator iter = W.begin(); iter < W.end(); iter++)
  {
    vertices.push_back((**iter).getStartVertex());
  }

  if (floor.isDefined())
  {
    floor.apply();
    glNormal3f(0.0, 0.0, 1.0);
    glBegin(GL_TRIANGLE_FAN);
    {
      for (std::vector<Point2D>::iterator iter = vertices.begin(); iter < vertices.end(); iter++)
      {
        glTexCoord2f((GLfloat)((*iter)[0] / 8), (GLfloat)((*iter)[1] / 8));
        glVertex3d((*iter)[0], (*iter)[1], F);
      }
    }
    glEnd();
  }
  if (ceiling.isDefined())
  {
    ceiling.apply();
    glNormal3f(0.0, 0.0, -1.0);
    glBegin(GL_TRIANGLE_FAN);
    {
      for (std::vector<Point2D>::reverse_iterator iter = vertices.rbegin(); iter != vertices.rend(); iter++)
      {
        glTexCoord2f((GLfloat)((*iter)[0] / 8), (GLfloat)((*iter)[1] / 8));
        glVertex3d((*iter)[0], (*iter)[1], C);
      }
    }
    glEnd();
  }
  else
  {
    if (STENCIL_ENABLED)
    {
      glDepthMask(GL_FALSE); glDisable(GL_DEPTH_TEST);

      GLint stencilRef;
      glGetIntegerv(GL_STENCIL_REF, &stencilRef);
      glStencilFunc(GL_EQUAL, stencilRef, 0xFFFF);
      glStencilOp(GL_KEEP, GL_KEEP, GL_INCR);
      
      glColor4d(0.0, 0.0, 0.0, 0.0);
      glBindTexture(GL_TEXTURE_2D, textureHandles[NO_TEXTURE]);

      glBegin(GL_TRIANGLE_FAN);
      {
        for (std::vector<Point2D>::reverse_iterator iter = vertices.rbegin(); iter != vertices.rend(); iter++)
        {
          glTexCoord2f((GLfloat)((*iter)[0] / 8), (GLfloat)((*iter)[1] / 8));
          glVertex3d((*iter)[0], (*iter)[1], C);
        }
      }
      glEnd();

      glStencilFunc(GL_EQUAL, stencilRef + 1, 0xFFFF);
      glStencilOp(GL_KEEP, GL_KEEP, GL_KEEP);

      glCallList(displayListHandles[SKYBOX_LIST]);

      glStencilFunc(GL_EQUAL, stencilRef + 1, 0xFFFF);
      glStencilOp(GL_KEEP, GL_KEEP, GL_DECR);
      
      glColor4d(0.0, 0.0, 0.0, 0.0);
      glBindTexture(GL_TEXTURE_2D, textureHandles[NO_TEXTURE]);

      glBegin(GL_TRIANGLE_FAN);
      {
        for (std::vector<Point2D>::reverse_iterator iter = vertices.rbegin(); iter != vertices.rend(); iter++)
        {
          glTexCoord2f((GLfloat)((*iter)[0] / 8), (GLfloat)((*iter)[1] / 8));
          glVertex3d((*iter)[0], (*iter)[1], C);
        }
      }
      glEnd();

      glStencilFunc(GL_EQUAL, stencilRef, 0xFFFF);
      glStencilOp(GL_KEEP, GL_KEEP, GL_KEEP);

      glDepthMask(GL_TRUE); glEnable(GL_DEPTH_TEST);
    }
  }


  std::vector<Drawable*> contents;
  contents.insert(contents.end(), entities.begin(), entities.end());
  contents.insert(contents.end(), P.begin(), P.end());
  contents.insert(contents.end(), projectiles.begin(), projectiles.end());
  std::sort(contents.begin(), contents.end(), CompareDrawableDepth(pos));

  for (std::vector<Drawable*>::iterator iter = contents.begin(); iter != contents.end(); iter++) 
  {
    if (typeid(**iter) != typeid(Player) || bouncesLeft != NUM_REFLECTIONS)
      (*iter)->render3D(pos, left, right);
  }
}

Vector2D Sector::resolveCollisions(Point3D pos3, Vector2D vel, double radius, double height, bool canStep, Collidable* other)
{
  if (vel.length() == 0) return vel;
  Point2D pos = pos3.collapseToXY();
  std::vector<Collidable*> collidableLines;
  getCollidables(pos3, vel, radius * 2, height, collidableLines, canStep, other);

  Collision firstHit = earliestCollision(collidableLines, pos, vel, radius);

  if (firstHit.t == NO_INTERSECTION)
    return vel;
  else if (checkIsValidPosition(collidableLines, firstHit.intersection, radius, other))
  {
    Collision secondHit = earliestCollision(collidableLines, firstHit.intersection, firstHit.slide, radius);

    if (secondHit.t == NO_INTERSECTION)
      return firstHit.intersection + firstHit.slide - pos;
    else if (checkIsValidPosition(collidableLines, secondHit.intersection, radius, other))
      return secondHit.intersection - pos;
    else return Vector2D(0,0);
  }
  else
    return Vector2D(0,0);
}

void Sector::getCollidables(Point3D pos, Vector2D vel, double radius, double height, std::vector<Collidable*>& result, bool canStep, Collidable* other)
{
  for (std::vector<Wall*>::iterator iter = W.begin(); iter < W.end(); iter++)
  {
    (*iter)->getCollidables(pos, vel, radius, height, result, canStep, other); 
  }
  for (std::set<Entity*>::iterator iter = entities.begin(); iter != entities.end(); iter++)
  {
    result.push_back(*iter);
  }
}

void Sector::getEntities(Point2D pos, double radius, std::vector<Entity*>& result)
{
  for (std::vector<Wall*>::iterator iter = W.begin(); iter < W.end(); iter++)
  {
    (*iter)->getEntities(pos, radius, result); 
  }
  for (std::set<Entity*>::iterator iter = entities.begin(); iter != entities.end(); iter++)
  {
    if (((*iter)->getPosition().collapseToXY() - pos).length() < radius)
      result.push_back(*iter);
  }
}

void Sector::nearestHeights(Point2D pos, double radius, int& floor, int& ceiling, Collidable* other)
{
  floor = std::max(floor, F);
  ceiling = std::min(ceiling, C);

  for (std::vector<Wall*>::iterator iter = W.begin(); iter < W.end(); iter++)
  {
    (*iter)->nearestHeights(pos, radius, floor, ceiling, other);
  }
}

Sector* Sector::getDestSector(Point2D start, Point3D& end)
{
  for (std::vector<Wall*>::iterator iter = W.begin(); iter < W.end(); iter++)
  {
    Sector* result = (*iter)->walkThrough(start, end);
    if (result) return result->getDestSector(start, end);
  }
  return this;
}

Vector3D Sector::findEntity(Point2D pos, Point2D left, Point2D right, Entity* target, unsigned int portalDepth)
{
  for(std::set<Entity*>::iterator iter = entities.begin(); iter != entities.end(); iter++)
  {
    if ((*iter) == target && (*iter)->isVisible(pos, left, right))
    {
        Vector2D xy =(**iter).getPosition().collapseToXY() - pos;
        return Vector3D(xy[0], xy[1], 0) + Vector3D(0, 0, (*iter)->getPosition()[2] - F);
    }
  }

  for (std::vector<Wall*>::iterator iter = W.begin(); iter != W.end(); iter++)
  {
    Vector3D result = (*iter)->findEntity(pos, left, right, target, portalDepth);
    if (result[0] != 0 || result[1] != 0) return result;
  }

  return Vector3D(0,0,0);
}

std::ostream& Sector::operator<<(std::ostream& out)
{
  out << "Addr: " << this << " Floor:" << F << " Ceiling: " << C << std::endl;
  for (std::vector<Wall*>::iterator iter = W.begin(); iter < W.end(); iter++)
  {
    out << " |-";
    (**iter) << out;
    out << std::endl;
  }
  return out;
}

int Sector::floorHeight()
{
  return F;
}

int Sector::ceilingHeight()
{
  return C;
}

void Sector::addEntity(Entity *e)
{
  if (entities.find(e) != entities.end()) std::cerr << "Adding entity twice OSHT" << std::endl;
  entities.insert(e);
}

void Sector::removeEntity(Entity *e)
{
  if (entities.find(e) == entities.end()) std::cerr << "Removing nonexistant entity OSHT" << std::endl;
  entities.erase(entities.find(e));
}

void Sector::update(double time)
{
  {
    std::vector<ParticleBase*>::iterator pariter = P.begin();
    while (pariter != P.end())
    {
      (*pariter)->update(time);
      if ((*pariter)->isDead())
      {
        delete *pariter;
        pariter = P.erase(pariter);
      }
      else pariter++;
    }
  }
  {
    std::vector<ProjectileBase*>::iterator proiter = projectiles.begin();
    while (proiter != projectiles.end())
    {
      if ((*proiter)->isDead())
      {
        delete *proiter;
        proiter = projectiles.erase(proiter);
      }
      else proiter++;
    }
  }
}

void Sector::addParticle(ParticleBase* p)
{
  P.push_back(p);
}

HitscanResult Sector::resolveHitscan(Point3D source, Vector3D trace, bool hitEntities, bool crossTeleportals, double& tVal, Point3D& dest, Sector* &camSector, void*& victim)
{
  if (camSector != NULL) throw "Something ain't right here";
  Entity* closestEntity = NULL;
  double closestEntityT = NO_INTERSECTION;
  for (std::set<Entity*>::iterator iter = entities.begin(); iter != entities.end(); iter++)
  {
    double entityT = (*iter)->resolveHitscan(source, trace);
    if (entityT < closestEntityT)
    {
      closestEntityT = entityT;
      closestEntity = *iter;
    }
  }

  if (trace[2] != 0)
  {
    if (trace[2] > 0)
    {
      double t = ((C - source[2]) / trace[2]);
      if (t <= 1 && (closestEntityT == NO_INTERSECTION || t < closestEntityT))
      {
        Point3D hit = source + (t - EPSILON) * trace;
        bool inside = true;
        for (std::vector<Wall*>::iterator iter = W.begin(); iter != W.end(); iter++)
        {
          if (!(*iter)->isInside(hit.collapseToXY()))
          {
            inside = false;
            break;
          }
        }
        if (inside)
        {
          camSector = this;
          dest = hit;
          victim = &C;
          tVal = t;
          return CEILING;
        }
      }
    }
    else
    {
      double t = ((F - source[2]) / trace[2]);
      if (t <= 1 && (closestEntityT == NO_INTERSECTION || t < closestEntityT))
      {
        Point3D hit = source + (t - EPSILON) * trace;
        bool inside = true;
        for (std::vector<Wall*>::iterator iter = W.begin(); iter != W.end(); iter++)
        {
          if (!(*iter)->isInside(hit.collapseToXY()))
          {
            inside = false;
            break;
          }
        }
        if (inside)
        {
          camSector = this;
          dest = hit;
          victim = &F;
          tVal = t;
          return FLOOR;
        }
      }
    }
  }

  if (closestEntityT != NO_INTERSECTION)
  {
    Point3D hit = source + closestEntityT * trace;
    dest = hit;
    camSector = this;
    victim = closestEntity;
    tVal = closestEntityT;
    return ENTITY;
  }

  for (std::vector<Wall*>::iterator iter = W.begin(); iter != W.end(); iter++)
  {
    HitscanResult temp = (*iter)->resolveHitscan(source, trace, hitEntities, crossTeleportals, tVal, dest, camSector, victim);
    if (temp != NONE) return temp;
  }

  if (camSector == NULL)
  {
    camSector = this;
    dest = source + (1 - EPSILON) * trace;
    victim = NULL;
    tVal = 1;
    return NONE;
  }
  else
  {
    return NONE;
  }
}

Colour Sector::getFloorColour()
{
  return floor.getColour();
}

Colour Sector::getCeilingColour()
{
  return ceiling.getColour();
}

void Sector::addProjectile(ProjectileBase* p)
{
  projectiles.push_back(p);
}

void Sector::removeProjectile(ProjectileBase* p)
{
  std::vector<ProjectileBase*>::iterator iter = projectiles.begin();
  while((*iter) != p) iter++;
  projectiles.erase(iter);
}

void Sector::getProjectiles(std::vector<ProjectileBase*>& result)
{
  result.insert(result.end(), projectiles.begin(), projectiles.end());
}

bool Sector::hasSkyRoof()
{
  return !ceiling.isDefined();
}

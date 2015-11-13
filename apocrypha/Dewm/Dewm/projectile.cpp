#include "projectile.hpp"
#include "sector.hpp"
#include "misc.hpp"
#include "dlist.hpp"

ProjectileBase::ProjectileBase(Point3D pos, Vector3D vel, Sector* cs, bool useGrav)
: position(pos), velocity(vel), currSector(cs), isGravitated(useGrav), hasHit(false)
{
}

ProjectileBase::~ProjectileBase()
{
}

void ProjectileBase::update(double t)
{
  onUpdate(t);
  Point3D dest;
  Sector* destSector = NULL;
  void* victim;
  double dontCare;
  HitscanResult result = currSector->resolveHitscan(position, t * velocity, true, true, dontCare, dest, destSector, victim);
  currSector->removeProjectile(this);
  currSector = destSector;
  currSector->addProjectile(this);
  if (result != NONE) hasHit = true;
  if (result == WALL) wallCollide(dest, (Line*)victim);
  if (result == FLOOR) floorCollide(dest);
  if (result == CEILING) ceilingCollide(dest);
  if (result == ENTITY)
    entityCollide(dest, (Entity*)victim);

  position = dest;
  if (isGravitated) velocity = velocity + t * Vector3D(0, 0, -GRAVITY);
}

bool ProjectileBase::isDead()
{
  return hasHit;
}

Point3D ProjectileBase::getPosition() const
{
  return position;
}

bool ProjectileBase::drawFirst() const
{
  return false;
}

Rocket::Rocket(Point3D pos, Vector3D vel, Sector *cs):ProjectileBase(pos, vel, cs, false)
{
}

Rocket::~Rocket()
{
}

void Rocket::render3D(Point2D pos, Point2D left, Point2D right)
{
  Vector2D velFlat = velocity.collapseToXY();
  double zAngle = atan2(velocity[1], velocity[0]);
  double yAngle = atan2(velocity[2], velFlat.length());

  glPushMatrix();
  glTranslated(position[0], position[1], position[2]);
  glRotated(zAngle*180.0/M_PI, 0, 0, 1);
  glRotated(yAngle*-180.0/M_PI, 0, 1, 0);
  glCallList(displayListHandles[MISSILE_LIST]);
  glPopMatrix();
}

void Rocket::explode()
{
  std::vector<Entity*> victims;
  currSector->getEntities(position.collapseToXY(), 10, victims);

  for (std::vector<Entity*>::iterator iter = victims.begin(); iter != victims.end(); iter++)
  {
    double dist = (position - (*iter)->getPosition()).length();
    (*iter)->damage(std::max(0.0, 0.1 * (10.0 - dist)));
  }
}

void Rocket::ceilingCollide(Point3D pos)
{
  if (currSector->hasSkyRoof()) return;
  currSector->addParticle(new BoomParticle(pos, 1, 10, Colour(1.0, 0.5, 0.5), Colour(0.0,0.0,0.0)));
  currSector->addParticle(new BulletHole(BulletHole::CEILING, pos, Vector3D(0, 0, -1), 10, 2, Colour(0.0, 0.0, 0.0)));
  explode();
}
void Rocket::floorCollide(Point3D pos)
{
  currSector->addParticle(new BoomParticle(pos, 1, 10, Colour(1.0, 0.5, 0.5), Colour(0.0,0.0,0.0)));
  currSector->addParticle(new BulletHole(BulletHole::FLOOR, pos, Vector3D(0, 0, 1), 10, 2, Colour(0.0, 0.0, 0.0)));
  explode();
}
void Rocket::wallCollide(Point3D pos, Line *wall)
{
  currSector->addParticle(new BoomParticle(pos, 1, 10, Colour(1.0, 0.5, 0.5), Colour(0.0,0.0,0.0)));
  currSector->addParticle(new BulletHole(BulletHole::WALL, pos, wall->getNormal(), 10, 2, Colour(0.0, 0.0, 0.0)));
  explode();
}
void Rocket::entityCollide(Point3D pos, Entity *victim)
{
  currSector->addParticle(new BoomParticle(pos, 1, 10, Colour(1.0, 0.5, 0.5), Colour(0.0,0.0,0.0)));
  explode();
}

void Rocket::onUpdate(double delta)
{
  smokeTime -= delta;
  if (smokeTime <= 0)
  {
    smokeTime = 0.025;
    Vector3D backdir = velocity;
    backdir.normalize();
    Vector3D updir(0.0, 0.0, 1.0);
    Vector3D sidedir = backdir.cross(updir);
    sidedir.normalize();
    currSector->addParticle(new QuakeParticle(
      position + -1 *backdir,
      -5 * backdir + (5 - 10 * mathRandom()) * sidedir + (5 - 10 * mathRandom()) * updir,
      0.25,
      Colour(1.0, 0.75, 0.0), false));
    currSector->addParticle(new QuakeParticle(
      position + -1 *backdir,
      -5 * backdir + (5 - 10 * mathRandom()) * sidedir + (5 - 10 * mathRandom()) * updir,
      0.25,
      Colour(0.2, 0.2, 0.2), false));
  }
}

Fireball::Fireball(Point3D pos, Vector3D vel, Sector *cs):ProjectileBase(pos, vel, cs, false), age(0.0)
{
}

Fireball::~Fireball()
{
}

void Fireball::render3D(Point2D pos, Point2D left, Point2D right)
{
  Vector2D velFlat = velocity.collapseToXY();
  double zAngle = atan2(velocity[1], velocity[0]);
  double yAngle = atan2(velocity[2], velFlat.length());

  glPushMatrix();
  glTranslated(position[0], position[1], position[2]);
  glRotated(zAngle*180.0/M_PI, 0, 0, 1);
  glRotated(yAngle*-180.0/M_PI, 0, 1, 0);
  glRotated(720 * age, 1, 0, 0);
  glDisable(GL_LIGHTING);
  glCallList(displayListHandles[FIREBALL_LIST]);
  glEnable(GL_LIGHTING);
  glPopMatrix();
}

void Fireball::ceilingCollide(Point3D pos)
{
  if (currSector->hasSkyRoof()) return;
  currSector->addParticle(new BoomParticle(pos, 0.5, 2, Colour(1.0, 0.5, 0.0), Colour(0.0,0.0,0.0)));
  currSector->addParticle(new BulletHole(BulletHole::CEILING, pos, Vector3D(0, 0, -1), 10, 0.5, Colour(0.0, 0.0, 0.0)));
}
void Fireball::floorCollide(Point3D pos)
{
  currSector->addParticle(new BoomParticle(pos, 0.5, 2, Colour(1.0, 0.5, 0.0), Colour(0.0,0.0,0.0)));
  currSector->addParticle(new BulletHole(BulletHole::FLOOR, pos, Vector3D(0, 0, 1), 10, 0.5, Colour(0.0, 0.0, 0.0)));
}
void Fireball::wallCollide(Point3D pos, Line *wall)
{
  currSector->addParticle(new BoomParticle(pos, 0.5, 2, Colour(1.0, 0.5, 0.0), Colour(0.0,0.0,0.0)));
  currSector->addParticle(new BulletHole(BulletHole::WALL, pos, wall->getNormal(), 10, 0.5, Colour(0.0, 0.0, 0.0)));
}
void Fireball::entityCollide(Point3D pos, Entity *victim)
{
  currSector->addParticle(new BoomParticle(pos, 0.5, 2, Colour(1.0, 0.5, 0.0), Colour(0.0,0.0,0.0)));
  victim->damage(0.08 + mathRandom() * 0.02);
}

void Fireball::onUpdate(double delta)
{
  age += delta;
  smokeTime -= delta;
  if (smokeTime <= 0)
  {
    smokeTime = 0.025;
    Vector3D backdir = velocity;
    backdir.normalize();
    Vector3D updir(0.0, 0.0, 1.0);
    Vector3D sidedir = backdir.cross(updir);
    sidedir.normalize();
    currSector->addParticle(new QuakeParticle(
      position + -0.5 *backdir,
      -5 * backdir + (5 - 10 * mathRandom()) * sidedir + (5 - 10 * mathRandom()) * updir,
      0.25,
      Colour(1.0, 0.5, 0.0), false));
    currSector->addParticle(new QuakeParticle(
      position + -0.5 *backdir,
      -5 * backdir + (5 - 10 * mathRandom()) * sidedir + (5 - 10 * mathRandom()) * updir,
      0.25,
      Colour(1.0, 0.0, 0.0), false));
  }
}

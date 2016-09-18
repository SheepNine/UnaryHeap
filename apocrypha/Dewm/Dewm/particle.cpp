#include "particle.hpp"
#include "misc.hpp"
#include "texture.hpp"

ParticleBase::ParticleBase(Point3D p, Vector3D v, double t, bool g)
: position(p), velocity(v), timeLeft(t), isGravitied(g)
{
}

ParticleBase::~ParticleBase() {}

Point3D ParticleBase::getPosition() const
{
  return position;
}

void ParticleBase::update(double t)
{
  position = position + t * velocity;
  if (isGravitied)
  {
    position = position + t * t * Vector3D(0, 0, -GRAVITY);
    velocity = velocity + t * Vector3D(0, 0, -GRAVITY);
  }
  timeLeft -= t;
}
bool ParticleBase::isDead()
{
  return timeLeft <= 0;
}

QuakeParticle::QuakeParticle(Point3D p, Vector3D v, double t, Colour c) : ParticleBase(p, v, t, true), colour(c), initialTime(t)
{
}
QuakeParticle::QuakeParticle(Point3D p, Vector3D v, double t, Colour c, bool g) : ParticleBase(p, v, t, g), colour(c), initialTime(t)
{
}

QuakeParticle::~QuakeParticle() {}

void QuakeParticle::render3D(Point2D pos, Point2D left, Point2D right)
{
  glDisable(GL_LIGHTING);
  glColor4d(colour.R(), colour.G(), colour.B(), timeLeft / initialTime);
  glBindTexture(GL_TEXTURE_2D, textureHandles[NO_TEXTURE]);
  double size = std::max(1.0, 10 - (position.collapseToXY() - pos).length() / 5);
  glPointSize((GLfloat)size);

  glBegin(GL_POINTS);
  {
    glVertex3d(position[0], position[1], position[2]);
  }
  glEnd();
  glEnable(GL_LIGHTING);
}

bool QuakeParticle::drawFirst() const { return false; }

TraceParticle::TraceParticle(Point3D p, Vector3D v, double t, Colour c) : ParticleBase(p, v, t, true), colour(c), initialTime(t)
{
}

TraceParticle::~TraceParticle() {}

void TraceParticle::render3D(Point2D pos, Point2D left, Point2D right)
{
  glDisable(GL_LIGHTING);
  glBindTexture(GL_TEXTURE_2D, textureHandles[NO_TEXTURE]);

  Vector3D traceLength = 0.025 * velocity;
  Point3D front = position + traceLength;
  Point3D back = position - traceLength;

  glLineWidth(3.0f);
  glBegin(GL_LINES);
  {
    glColor4d(colour.R(), colour.G(), colour.B(), timeLeft / initialTime);
    glVertex3d(front[0], front[1], front[2]);
    glColor4d(colour.R(), colour.G(), colour.B(), 0.0);
    glVertex3d(back[0], back[1], back[2]);
  }
  glEnd();
  glEnable(GL_LIGHTING);
  glLineWidth(1.0f);
}

bool TraceParticle::drawFirst() const { return false; }

BoomParticle::BoomParticle(Point3D p, double t, double ms, Colour s, Colour e)
: ParticleBase(p, Vector3D(0,0,0), t, false), initialTime(t), maxSize(ms), start(s), end(e)
{
}

BoomParticle::~BoomParticle() {}

void BoomParticle::render3D(Point2D pos, Point2D left, Point2D right)
{
  double percent = timeLeft / initialTime;
  Colour currColour = percent * start + (1-percent) * end;
  glColor4d(currColour.R(), currColour.G(), currColour.B(), percent);
  glBindTexture(GL_TEXTURE_2D, textureHandles[NO_TEXTURE]);
  double size = maxSize * (1-percent);

  glDisable(GL_LIGHTING); glDepthMask(GL_FALSE);

  glTranslated(position[0], position[1], position[2]);
  GLUquadric* q = gluNewQuadric();
  gluSphere(q, size, 8, 8);
  gluDeleteQuadric(q);
  glTranslated(-position[0], -position[1], -position[2]);

  glEnable(GL_LIGHTING); glDepthMask(GL_TRUE);
}

bool BoomParticle::drawFirst() const { return false; }

RicochetParticle::RicochetParticle(Point3D p, double t): ParticleBase(p, Vector3D(0,0,0), t, false), initialTime(t)
{
}
RicochetParticle::~RicochetParticle() {}

void RicochetParticle::render3D(Point2D pos, Point2D left, Point2D right)
{
  double percent = timeLeft / initialTime;
  glColor4d(1.0, 0.75, 0.13, percent);
  glBindTexture(GL_TEXTURE_2D, textureHandles[NO_TEXTURE]);
  double size = 0.15;

  glDisable(GL_LIGHTING);
  glPushMatrix();
  glTranslated(position[0], position[1], position[2]);
  glBegin(GL_TRIANGLES);
  {
    glVertex3d(0, size, 0); glVertex3d(0, 0, size); glVertex3d(size, 0, 0);
    glVertex3d(0, size, 0); glVertex3d(-size, 0, 0); glVertex3d(0, 0, size);
    glVertex3d(-size, 0, 0); glVertex3d(0, -size, 0); glVertex3d(0, 0, size);
    glVertex3d(size, 0, 0); glVertex3d(0, 0, size); glVertex3d(0, -size, 0);

    glVertex3d(0, size, 0); glVertex3d(size, 0, 0); glVertex3d(0, 0, -size);
    glVertex3d(0, size, 0); glVertex3d(0, 0, -size); glVertex3d(-size, 0, 0);
    glVertex3d(-size, 0, 0); glVertex3d(0, 0, -size); glVertex3d(0, -size, 0);
    glVertex3d(size, 0, 0); glVertex3d(0, -size, 0); glVertex3d(0, 0, -size);
  }
  glEnd();
  glPopMatrix();
  glEnable(GL_LIGHTING);
}

bool RicochetParticle::drawFirst() const { return false; }

BulletHole::BulletHole(BulletHole::Surface s, Point3D p, Vector3D n, double t, double big, Colour c)
:ParticleBase(p, Vector3D(0,0,0), t, false), surface(s), size(big), color(c)
{
  normal = n;
  normal.normalize();
  normal = 0.1 * normal;
}
BulletHole::~BulletHole() {}

void BulletHole::render3D(Point2D pos, Point2D left, Point2D right)
{
  Point3D bl, br, tl, tr;
  if (surface == FLOOR)
  {
    bl = Point3D(-size, -size, 0);
    br = Point3D(size, -size, 0);
    tr = Point3D(size, size, 0);
    tl = Point3D(-size, size, 0);
  }
  else if (surface == CEILING)
  {
    bl = Point3D(-size, -size, 0);
    br = Point3D(-size, size, 0);
    tr = Point3D(size, size, 0);
    tl = Point3D(size, -size, 0);
  }
  else if (surface == WALL)
  {
    bl = Point3D(0.0, -size, -size);
    br = Point3D(0.0, size, -size);
    tr = Point3D(0.0, size, size);
    tl = Point3D(0.0, -size, size);
  }

  glDisable(GL_LIGHTING); glDepthMask(GL_FALSE);
  glPushMatrix();
  glTranslated(position[0], position[1], position[2]);
  glTranslated(normal[0], normal[1], normal[2]);
  if (surface == WALL)
  {
    double angle = atan2(normal[1], normal[0]) * 180 / M_PI;
    glRotated(angle, 0, 0, 1);
  }

  glColor3d(color.R(), color.G(), color.B());
  glBindTexture(GL_TEXTURE_2D, textureHandles[BHOLE_TEXTURE]);
  glBegin(GL_QUADS);
  {
    glTexCoord2d(0.0, 0.0);
    glVertex3d(bl[0], bl[1], bl[2]);
    glTexCoord2d(1.0, 0.0);
    glVertex3d(br[0], br[1], br[2]);
    glTexCoord2d(1.0, 1.0);
    glVertex3d(tr[0], tr[1], tr[2]);
    glTexCoord2d(0.0, 1.0);
    glVertex3d(tl[0], tl[1], tl[2]);
  }
  glEnd();

  glPopMatrix();
  glEnable(GL_LIGHTING); glDepthMask(GL_TRUE);
}

bool BulletHole::drawFirst() const { return true; }
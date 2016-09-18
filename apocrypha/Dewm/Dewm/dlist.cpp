#include "dlist.hpp"
#include "texture.hpp"

GLuint displayListHandles[NUM_DISPLAY_LISTS];

void createSkyboxList(GLuint handle)
{
  /*GLdouble brightSky[3] = {0.5, 0.75, 1.0};
  GLdouble medSky[3] = {0.0, 0.5, 1.0};
  GLdouble darkSky[3] = {0.0, 0.0, 1.0};
  GLdouble darkerSky[3] = {0.0, 0.0, 0.5};*/
  GLdouble brightSky[3] = {0.75, 0.88, 1.0};
  GLdouble medSky[3] = {0.0, 0.5, 1.0};
  GLdouble darkSky[3] = {0.0, 0.0, 1.0};
  GLdouble darkerSky[3] = {0.0, 0.0, 0.5};
  GLdouble sun[4] = {1.0, 1.0, 0.5};
  GLdouble brightSun[4] = {1.0, 1.0, 1.0};

  glNewList(handle, GL_COMPILE);
  {
    glDisable(GL_LIGHTING);
    glBindTexture(GL_TEXTURE_2D, textureHandles[NO_TEXTURE]);

    glBegin(GL_TRIANGLES);
    {
      // TOP
      glColor3dv(darkSky);
      glVertex3d(-1000.0, -1000.0, 1000.0);
      glColor3dv(medSky);
      glVertex3d(-1000.0, 1000.0, 1000.0);
      glColor3dv(brightSky);
      glVertex3d(1000.0, 1000.0, 1000.0);

      glColor3dv(darkSky);
      glVertex3d(-1000.0, -1000.0, 1000.0);
      glColor3dv(brightSky);
      glVertex3d(1000.0, 1000.0, 1000.0);
      glColor3dv(medSky);
      glVertex3d(1000.0, -1000.0, 1000.0);

      // SIDE A

      glColor3dv(brightSky);
      glVertex3d(1000.0, 1000.0, 1000.0);
      glColor3dv(medSky);
      glVertex3d(-1000.0, 1000.0, 1000.0);
      glColor3dv(darkSky);
      glVertex3d(-1000.0, 1000.0, -1000.0);

      glColor3dv(brightSky);
      glVertex3d(1000.0, 1000.0, 1000.0);
      glColor3dv(darkSky);
      glVertex3d(-1000.0, 1000.0, -1000.0);
      glColor3dv(medSky);
      glVertex3d(1000.0, 1000.0, -1000.0);

      // SIDE B


      glColor3dv(darkSky);
      glVertex3d(-1000.0, -1000.0, 1000.0);
      glColor3dv(darkerSky);
      glVertex3d(-1000.0, -1000.0, -1000.0);
      glColor3dv(medSky);
      glVertex3d(-1000.0, 1000.0, 1000.0);

      glColor3dv(darkSky);
      glVertex3d(-1000.0, 1000.0, -1000.0);
      glColor3dv(medSky);
      glVertex3d(-1000.0, 1000.0, 1000.0);
      glColor3dv(darkerSky);
      glVertex3d(-1000.0, -1000.0, -1000.0);

      // SIDE C

      glColor3dv(darkerSky);
      glVertex3d(-1000.0, -1000.0, -1000.0);
      glColor3dv(darkSky);
      glVertex3d(-1000.0, -1000.0, 1000.0);
      glColor3dv(medSky);
      glVertex3d(1000.0, -1000.0, 1000.0);

      glColor3dv(medSky);
      glVertex3d(1000.0, -1000.0, 1000.0);
      glColor3dv(darkSky);
      glVertex3d(1000.0, -1000.0, -1000.0);
      glColor3dv(darkerSky);
      glVertex3d(-1000.0, -1000.0, -1000.0);


      // SIDE D

      glColor3dv(brightSky);
      glVertex3d(1000.0, 1000.0, 1000.0);
      glColor3dv(medSky);
      glVertex3d(1000.0, 1000.0, -1000.0);
      glColor3dv(darkSky);
      glVertex3d(1000.0, -1000.0, -1000.0);

      glColor3dv(darkSky);
      glVertex3d(1000.0, -1000.0, -1000.0);
      glColor3dv(medSky);
      glVertex3d(1000.0, -1000.0, 1000.0);
      glColor3dv(brightSky);
      glVertex3d(1000.0, 1000.0, 1000.0);
    }
    glEnd();

    glPushMatrix();
    glTranslated(900.0, 900.0, 900.0);
    glRotated(-45, 0, 0, 1);
    glRotated(45, 1, 0, 0);
    glBegin(GL_TRIANGLE_FAN);
    {
      glColor3dv(brightSun);
      glVertex3d(0.0, 0.0, 0.0);
      glColor3dv(sun);
      for (int count = 0; count < 17; count++)
      {
        glVertex3d(50 * cos(2 * M_PI * count / 16.0), 0, 50 * sin(2 * M_PI * count / 16.0));
      }
    }
    glEnd();
    glPopMatrix();

    glEnable(GL_LIGHTING);
  }
  glEndList();
}

void createMissileList(GLuint handle)
{
  glNewList(handle, GL_COMPILE);
  {
    glBindTexture(GL_TEXTURE_2D, textureHandles[NO_TEXTURE]);
    glBegin(GL_TRIANGLES);
    {
      glColor3d(0.0, 0.5, 0.0);
      glNormal3d(0.0, -1.0, 1.0);
      glVertex3d(1.0, 0.0, 0.0); glVertex3d(0.5, 0.25, 0.0); glVertex3d(0.5, 0.0, 0.25);
      glNormal3d(0.0, -1.0, -1.0);
      glVertex3d(1.0, 0.0, 0.0); glVertex3d(0.5, 0.0, -0.25); glVertex3d(0.5, 0.25, 0.0);
      glNormal3d(0.0, 1.0, -1.0);
      glVertex3d(1.0, 0.0, 0.0); glVertex3d(0.5, -0.25, 0.0); glVertex3d(0.5, 0.0, -0.25);
      glNormal3d(0.0, 1.0, 1.0);
      glVertex3d(1.0, 0.0, 0.0); glVertex3d(0.5, 0.0, 0.25); glVertex3d(0.5, -0.25, 0.0);

      glColor3d(0.75, 0.75, 0.75);
      glNormal3d(0.0, -1.0, 1.0);
      glVertex3d(-1.0, 0.0, 0.25); glVertex3d(-1.0, -0.25, 0.0); glVertex3d(0.5, 0.0, 0.25);
      glVertex3d(0.5, 0.0, 0.25); glVertex3d(-1.0, -0.25, 0.0); glVertex3d(0.5, -0.25, 0.0);
      glNormal3d(0.0, -1.0, -1.0);
      glVertex3d(-1.0, -0.25, 0.0); glVertex3d(-1.0, 0.0, -0.25); glVertex3d(0.5, -0.25, 0.0);
      glVertex3d(0.5, -0.25, 0.0); glVertex3d(-1.0, 0.0, -0.25); glVertex3d(0.5, 0.0, -0.25);
      glNormal3d(0.0, 1.0, -1.0);
      glVertex3d(-1.0, 0.0, -0.25); glVertex3d(0.5, 0.25, 0.0); glVertex3d(0.5, 0.0, -0.25);
      glVertex3d(0.5, 0.25, 0.0); glVertex3d(-1.0, 0.0, -0.25); glVertex3d(-1.0, 0.25, 0.0);
      glNormal3d(0.0, 1.0, 1.0);
      glVertex3d(0.5, 0.25, 0.0);  glVertex3d(-1.0, 0.25, 0.0); glVertex3d(-1.0, 0.0, 0.25);
      glVertex3d(0.5, 0.25, 0.0); glVertex3d(-1.0, 0.0, 0.25); glVertex3d(0.5, 0.0, 0.25);
    }
    glEnd();
    glDisable(GL_LIGHTING);
    glBegin(GL_TRIANGLES);
    {
      glColor3d(1.0, 0.75, 0.0);
      glNormal3d(-1.0, 0.0, 0.0);
      glVertex3d(-1.0, 0.0, 0.25); glVertex3d(-1.0, 0.0, -0.25); glVertex3d(-1.0, -0.25, 0.0);
      glVertex3d(-1.0, 0.0, -0.25); glVertex3d(-1.0, 0.0, 0.25); glVertex3d(-1.0, 0.25, 0.0);
    }
    glEnd();
    glEnable(GL_LIGHTING);
  }
  glEndList();
}

void createFireballList(GLuint handle)
{
  double width = 0.2;
  Colour side(1.0, 0.0, 0.0);
  Colour front(1.0, 0.5, 0.0);
  Colour back(0.0, 0.0, 0.0);
  glNewList(handle, GL_COMPILE);
  {
    glBindTexture(GL_TEXTURE_2D, textureHandles[NO_TEXTURE]);
    glBegin(GL_TRIANGLES);
    {
      glNormal3d(1.0, 1.0, 1.0);
      glColor3d(front.R(), front.G(), front.B());
      glVertex3d(0.5, 0.0, 0.0);
      glColor3d(side.R(), side.G(), side.B());
      glVertex3d(0.0, width, 0.0);
      glVertex3d(0.0, 0.0, width);

      glNormal3d(1.0, 1.0, -1.0);
      glColor3d(front.R(), front.G(), front.B());
      glVertex3d(0.5, 0.0, 0.0);
      glColor3d(side.R(), side.G(), side.B());
      glVertex3d(0.0, 0.0, -width);
      glVertex3d(0.0, width, 0.0);

      glNormal3d(1.0, -1.0, -1.0);
      glColor3d(front.R(), front.G(), front.B());
      glVertex3d(0.5, 0.0, 0.0);
      glColor3d(side.R(), side.G(), side.B());
      glVertex3d(0.0, -width, 0.0);
      glVertex3d(0.0, 0.0, -width);

      glNormal3d(1.0, -1.0, 1.0);
      glColor3d(front.R(), front.G(), front.B());
      glVertex3d(0.5, 0.0, 0.0);
      glColor3d(side.R(), side.G(), side.B());
      glVertex3d(0.0, 0.0, width);
      glVertex3d(0.0, -width, 0.0);


      glNormal3d(-1.0, 1.0, 1.0);
      glColor3d(back.R(), back.G(), back.B());
      glVertex3d(-1.0, 0.0, 0.0);
      glColor3d(side.R(), side.G(), side.B());
      glVertex3d(0.0, 0.0, width);
      glVertex3d(0.0, width, 0.0);     

      glNormal3d(-1.0, 1.0, -1.0);
      glColor3d(back.R(), back.G(), back.B());
      glVertex3d(-1.0, 0.0, 0.0);
      glColor3d(side.R(), side.G(), side.B());
      glVertex3d(0.0, width, 0.0);
      glVertex3d(0.0, 0.0, -width);

      glNormal3d(-1.0, -1.0, -1.0);
      glColor3d(back.R(), back.G(), back.B());
      glVertex3d(-1.0, 0.0, 0.0);
      glColor3d(side.R(), side.G(), side.B());
      glVertex3d(0.0, 0.0, -width);
      glVertex3d(0.0, -width, 0.0);    

      glNormal3d(-1.0, -1.0, 1.0);
      glColor3d(back.R(), back.G(), back.B());
      glVertex3d(-1.0, 0.0, 0.0);
      glColor3d(side.R(), side.G(), side.B());
      glVertex3d(0.0, -width, 0.0);
      glVertex3d(0.0, 0.0, width);      
    }
    glEnd();
  }
  glEndList();
}

void createLists()
{
  for (int count = 0; count < NUM_DISPLAY_LISTS; count++)
    displayListHandles[count] = glGenLists(1);

  createSkyboxList(displayListHandles[SKYBOX_LIST]);
  createMissileList(displayListHandles[MISSILE_LIST]);
  createFireballList(displayListHandles[FIREBALL_LIST]);
}

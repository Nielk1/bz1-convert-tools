#include <conio.h>
#include <string.h>
#include <stdio.h>
#include <stdlib.h>

typedef unsigned char  byte;
typedef unsigned short word;

struct bitmap_header
{
    word type;
    word lof;
    word lof2;
    word x_hot;
    word y_hot;
    word first_pixel;
    word first_pixel2;
    word header_size;
    word header_size2;
    word x_size;             //10
    word x_size2;
    word y_size;
    word y_size2;
    word target;
    word bits_per_pixel;      //10
    
    word compression_method;
    word compression_method2;
    word compressed_size;
    word compressed_size2;
    word x_res;              //10
    word x_res2;
    word y_res;
    word y_res2;
    word used_clrs;
    word used_clrs2;         //10
    word important_clrs;
    word important_clrs2;
}bmfh;

struct colour //only important when saving. assumes b -> w greyscale on loading.
{
    unsigned char r,g,b;
};

struct palette
{
    struct colour col[256];
}pal;

byte * px;
int ix,iy;


int load_bmp8(char *fname)
{
    FILE *input;
    int i,skip;
    input=fopen(fname,"rb");
	if (input)
	{
		fread(&bmfh,54,1,input);
	    if (bmfh.bits_per_pixel==8)
	    {
	        for(i=0;i<256;i++)
	        {
				pal.col[i].b=(getc(input));
				pal.col[i].g=(getc(input));
				pal.col[i].r=(getc(input));
				skip=(getc(input));
	        }
			ix=bmfh.x_size;
			iy=bmfh.y_size;
	        px=(char *)malloc(bmfh.x_size*bmfh.y_size);
	        for (i=(bmfh.y_size-1);i>-1;i--)
	        {
	            fread(px+(i*bmfh.x_size),sizeof(char),bmfh.x_size,input);
	        }
		    fclose(input);
			return 1;
	    }
	    fclose(input);
	}
	return 0;
}


void save_lgt(char *fname)
{
   int x,y;
   int pos;                     
   int basepos;                
   char xsegs,ysegs;       
   signed char xseg,yseg;
   byte l;

   FILE *output;
   output=fopen(fname,"w+b");
   xsegs=ix/128;              
   ysegs=iy/128;
   printf("\n %d chunks(s) detected...\n Creating chunk ",xsegs*ysegs);

   l=px[0];
   for (x=0;x<128*128;x++)
   {
      putc(l,output);
   }
   for (yseg=(ysegs-1);yseg>-1;yseg--)                   
   {
      for (xseg=0;xseg<xsegs;xseg++)
      {
         basepos =(xseg*128)+(yseg*128*128*xsegs);
         printf("#");
         for (y=127;y>-1;y--)                            
         {
            for (x=0;x<128;x++)
            {
               pos=((y*ix)+x)+basepos;                 
               putc(px[pos],output);
            }
         }
      }
   }
   fclose(output);
}

int select(int min, int max)
{
	int k = getch()-48;
    if (k >= min && k<= max)
    {
        printf("\n %d selected\n",k);
		return k;
    }
    else
    {
         printf("\n invalid selection.. defaulting to option 1\n");
         return 1;
    }
}

int load_lgt(char *fname)
{
   int y;
   int pos;                     
   int basepos;                
   int segs =-2;
   int xsegs,ysegs;       
   signed char xseg,yseg;

   FILE *input;
	px=(byte*)malloc(128*128);
   input=fopen(fname,"rb");
   while (!feof( input))
   {
		fread( px,1,128*128,input);
		segs++;
   }
	printf("\n%i blocks detected\n", segs);
	switch (segs)
	{
	case 1:
		xsegs=1;
		ysegs=1;
		break;
	case 2:
		printf("\n Please select\n");
		printf("\n 1: 256 x 128");
		printf("\n 2: 128 x 256");
		ysegs=select(1,2);
		xsegs=3-ysegs;
		break;
	case 3:
		printf("\n Please select\n");
		printf("\n 1: 384 x 128");
		printf("\n 2: 128 x 384");
		ysegs=select(1,2);
		if (ysegs==2)ysegs=3;
		xsegs=4-ysegs;
		break;
	case 4:
		printf("\n Please select\n");
		printf("\n 1: 512 x 128");
		printf("\n 2: 256 x 256");
		printf("\n 3: 128 x 512");
		ysegs=select(1,3);
		if (ysegs==3)ysegs=4;
		xsegs=4/ysegs;
		break;
	case 6:
		printf("\n Please select\n");
		printf("\n 1: 384 x 256");
		printf("\n 2: 256 x 384");
		ysegs=select(1,2)+1;
		xsegs=5-ysegs;
		break;
	case 8:
		printf("\n Please select\n");
		printf("\n 1: 512 x 256");
		printf("\n 2: 256 x 512");
		ysegs=select(1,2)*2;
		xsegs=6-ysegs;
		break;
	case 9:
		xsegs=3;
		ysegs=3;
		break;
	case 12:
		printf("\n Please select\n");
		printf("\n 1: 512 x 384");
		printf("\n 2: 384 x 512");
		ysegs=select(1,2)+2;
		xsegs=7-ysegs;
	case 16:
		xsegs=4;
		ysegs=4;
		break;
	default:
		printf("\n Unable to process file\n");
		return 0;
		break;
	}
	printf("processing %i x %i blocks",xsegs,ysegs);

	ix=xsegs*128;
	iy=ysegs*128;

	free(px);
	px=malloc(128*128*segs);
	fseek(input,128*128,SEEK_SET);
   for (yseg=(ysegs-1);yseg>-1;yseg--)                   
   {
      for (xseg=0;xseg<xsegs;xseg++)
      {
         basepos =(xseg*128)+(yseg*128*128*xsegs);
         printf("#");
         for (y=127;y>-1;y--)                            
         {
            pos=(y*ix)+basepos;
				fread(px+pos,1,128,input);
         }
      }
   }
   fseek(input,1,SEEK_SET);
   px[0]=getc(input);
   fclose(input);
   return 1;
}



void make_bmfh(int w, int h)
{
	int c;
    int b;//buffer var
	 //greyscale palette
 	 for(c=0;c<=255;c++){
		 pal.col[c].r=c;
		 pal.col[c].g=c;
		 pal.col[c].b=c;
	 }

    //write BMP header...
    bmfh.type=(77<<8)+66;
    b=1078+(w*h);
    bmfh.lof=b&65535;
    bmfh.lof2=b>>16;
    bmfh.x_hot=0;
    bmfh.y_hot=0;
    bmfh.first_pixel=1078;
    bmfh.first_pixel2=0;
    bmfh.header_size=40;
    bmfh.header_size2=0;
    bmfh.x_size=w;
    bmfh.x_size2=0;
    bmfh.y_size=h;
    bmfh.y_size2=0;
    bmfh.target=1;
    bmfh.bits_per_pixel=8;
    
    bmfh.compression_method=0;
    bmfh.compression_method2=0;
    bmfh.compressed_size=0;
    bmfh.compressed_size2=0;
    bmfh.x_res=666;
    bmfh.x_res2=0;
    bmfh.y_res=666;
    bmfh.y_res2=0;
    bmfh.used_clrs=0;
    bmfh.used_clrs2=0;
    bmfh.important_clrs=0;
    bmfh.important_clrs2=0;
    
}

void save_bmp(char *fname)
{
    int i;
    FILE *output;
    output=fopen(fname,"w+b");
    fwrite(&bmfh,2,27,output);
    if (bmfh.bits_per_pixel==8)
    {
        for(i=0;i<256;i++)
        {
          putc((pal.col[i].b),output);
          putc((pal.col[i].g),output);
          putc((pal.col[i].r),output);
          putc(0,output);
        }
        for (i=(bmfh.y_size-1);i>-1;i--)
        {
            fwrite(px+(i*bmfh.x_size),1,bmfh.x_size,output);
        }
    }
//    if (bmfh.bits_per_pixel==24)
//    {
//        for (i=(bmfh.y_size-1);i>-1;i--)
//        {
//            fwrite(px+(3*i*bmfh.x_size),sizeof(char),3*bmfh.x_size,output);
//        }
//    }
    fclose(output);
}

void die(){
		printf("\n Error: Give me an 8 bit bmp or lgt file next time please\n");
		exit(0);
}

void main(int argc, char*argv[])
{
   char *fname; 
   char *output;
   int ok;
	printf("\n BzLgt - lightmap file convertor for Battlezone Editors\n");

   if(argc==1) die();
   fname = argv[1];
   output = strlwr(strdup(fname));

   if (strstr(strlwr(fname),".bmp"))
   {
      //convert bmp to lgt
      ok=load_bmp8(fname);
      strcpy(strstr(output,".bmp"),".lgt");
	  if (ok)
	  {
		  save_lgt(output);
	  }
   }
   
   if (strstr(strlwr(fname),".lgt"))
   {
      //convert lgt to bmp
      ok=load_lgt(fname);
      strcpy(strstr(output,".lgt"),".bmp");
	  if (ok)
	  {
		  make_bmfh(ix,iy);
		  save_bmp(output);
	  }
   }
}


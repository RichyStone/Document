#ifdef DLL_EXPORTS
#define XYZDLL_EXPORT extern "C" __declspec(dllexport)
#else
#define XYZDLL_EXPORT  extern "C" __declspec(dllimport)
#endif

XYZDLL_EXPORT void Thin_Fizeau_Cavity_2023Bv1(int elements, int Badpixels_n , int *Badpixels_p, double pixelsize, double Zero_piont,	double wedge_spacing,	
	                                                                                           double wedge_angle,	double Period_Zero, double *Intensity,double *F_results);

XYZDLL_EXPORT void Thick_Fizeau_Cavity_2023Bv1(int elements, int Badpixels_n , int *Badpixels_p, double pixelsize, double Zero_piont,	double wedge_spacing,	
	                                                                                           double wedge_angle,	double Period_Zero,double Lambda1, double *Intensity,double *F_results);

////
#ifdef DLL_EXPORTS
#define XYZDLL_EXPORT extern "C" __declspec(dllexport)
#else
#define XYZDLL_EXPORT  extern "C" __declspec(dllimport)
#endif


XYZDLL_EXPORT void Thin_Fizeau_Cavity_v20220726(double* Intensity, double* F_results,
	int elements, double pixelsize, double Zero_piont, double wedge_spacing,
	double wedge_angle,double Period_Zero);
XYZDLL_EXPORT void Thick_Fizeau_Cavity_v20220726(double* Intensity, double* F_results,
	int elements, double pixelsize, double Zero_piont, double wedge_spacing,
	double wedge_angle, double Period_Zero);
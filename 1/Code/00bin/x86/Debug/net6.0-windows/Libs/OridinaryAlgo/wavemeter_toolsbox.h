#ifdef DLL_EXPORTS
#define XYZDLL_EXPORT extern "C" __declspec(dllexport)
#else
#define XYZDLL_EXPORT  extern "C" __declspec(dllimport)
#endif

struct Wedge {
	int elements;
	double pixelsize;
	double Zero_piont;
	double wedge_spacing;
	double wedge_angle;
	double Period_Zero;
}; 

struct Calibration {
	int C_date;
	int C_time;
	int Max_n;
	double Lambda0;
	double Intercept;
	double Periods;
}; 

XYZDLL_EXPORT double round(double x);
XYZDLL_EXPORT double str2num(char *str);
XYZDLL_EXPORT void flip_Odd_Even(double *datap, int datalen);
XYZDLL_EXPORT void Odd_Even_Filter(double *Intensity, int datalen);
XYZDLL_EXPORT int Find_nodes(double *datap, int datalen, double *locationx, double *locationp, int Filterwidth);
XYZDLL_EXPORT int Find_Max_nodes(double *datap, int datalen, double *locationp, double *locationv, int Filterwidth);
XYZDLL_EXPORT void Linear_Fitting(int datalen, double *X, double *Y,double *results);
XYZDLL_EXPORT double Find_Width(double *datap, int datalen, double Max_val, double Max_p);
XYZDLL_EXPORT void Thin_Fizeau_Cavity(double *Intensity,double *F_results);
XYZDLL_EXPORT void Thick_Fizeau_Cavity(double *Intensity,double *F_results);
XYZDLL_EXPORT void Thin_Fizeau_Cavity_v0(double *Intensity,double *F_results, double *S_periods);
XYZDLL_EXPORT void Thin_Fizeau_Cavity_v1(double *Intensity,double *F_results);
XYZDLL_EXPORT void Thin_Fizeau_Cavity_cutv1(int start, int end, double *Intensity,double *F_results);
XYZDLL_EXPORT void Acquire_F0Calibration_data_v1(double Lambda0, double *Intensity,double *F_results);
XYZDLL_EXPORT void Calibration_Cavity_F0(char *Calibration_data_file, double step, int Cstepn);

XYZDLL_EXPORT void Thick_Fizeau_Cavity_v2(double Lambda1, double *Intensity,double *F_results);
XYZDLL_EXPORT void Thick_Fizeau_Cavity_v3(double Lambda1, double *Intensity,double *F_results);
///
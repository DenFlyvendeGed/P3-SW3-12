namespace P3_Project.Models.ReservationPdf;

public interface LatexReservationItem {}
public class LatexItemModel :LatexReservationItem {
	public string Id {get; set;} = "";
	public string Name {get; set;} = "";
	public int Amount {get; set;} = 0;
	public int IndividualPrice {get; set;} = 0;
	public int TotalPrice {get; set;} = 0;
	
	override public string ToString() => $"{Id} & {Name} & {Amount} & {IndividualPrice} & {TotalPrice}";
}
public class LatexPackModel : LatexItemModel {
	public List<LatexPackModelItem> Items {get; set;} = new();
	override public string ToString() => $"\\hline {Id} & {Name} & {Amount} & {IndividualPrice} & {TotalPrice} {string.Join("", Items)}";
}

public class LatexPackModelItem {
	public int Id {get; set;} = 0;
	public string Name {get; set;} = "";
	public int Amount {get; set;} = 0;
	override public string ToString() => "\\\\\\hline {Id} & {Name} & {Amount} &&";	
}

public class LATEX_GLOBALS {
	public static string TEX_STRING(string header, string name, string date, string id, string total, string salestax, IEnumerable<LatexReservationItem> items) {
		var table = string.Join("\\\\\\hline", items);
		return $@"
\documentclass[a4paper]{{article}}
\usepackage[a4paper, margin=2cm]{{geometry}}
\usepackage{{graphicx}}
\usepackage{{tabularx}}

% Colors
\usepackage[dvipsnames]{{xcolor}}
\usepackage{{colortbl}}
%% Primary
\definecolor{{ASH-Black}}      {{RGB}}{{  0,   0,   0}}
\definecolor{{ASH-Blue}}       {{RGB}}{{ 59, 146, 178}}
\definecolor{{ASH-LightBlue}}  {{RGB}}{{115, 174, 200}}
%% Contrast
\definecolor{{ASH-Red}}        {{RGB}}{{181,   4,  67}}
\definecolor{{ASH-LightRed}}   {{RGB}}{{194,  80,  82}}
\definecolor{{ASH-Orange}}     {{RGB}}{{244, 116,  71}}
\definecolor{{ASH-LightOrange}}{{RGB}}{{246, 141,  71}}
\definecolor{{ASH-Yellow}}     {{RGB}}{{250, 226,   0}}
\definecolor{{ASH-Green}}      {{RGB}}{{  0, 131,  72}}
\definecolor{{ASH-LightGreen}} {{RGB}}{{ 47, 180,  87}}
\definecolor{{ASH-Purple}}     {{RGB}}{{ 38,  85, 132}}
\definecolor{{ASH-LightPurple}}{{RGB}}{{ 80, 106, 149}}
%% Concept
\definecolor{{ASH-Turquoise}}  {{RGB}}{{131, 208, 245}}
\definecolor{{ASH-LightYellow}}{{RGB}}{{255, 254, 106}}
\definecolor{{ASH-Pink}}       {{RGB}}{{242, 159, 197}}


% Fonts
\usepackage{{fontspec}}
\newfontfamily\ashfranklin{{FranklinGothic}}[
	Path = fonts/FranklinGothicBook/,
	UprightFont = *BookRegular.ttf,
	ItalicFont = *BookItalic.ttf,
	BoldFont = *HeavyRegular.ttf,
	BoldItalicFont = *HeavyItalic.ttf
]

\begin{{document}}
	\thispagestyle{{empty}}
	%% Setup
	\ashfranklin{{}}
	\begin{{minipage}}{{0.4\textwidth}}
		{{\huge {header}}}\par\vspace{{1cm}}
	\end{{minipage}}\hspace{{\fill}}
	\begin{{minipage}}[t]{{0.4\textwidth}}
		\includegraphics[width=\textwidth]{{ash-logo.png}}
	\end{{minipage}}\par\vspace{{.5cm}}
	\textbf{{Kære {name}}},\par
	% Default top text
	Din reservation er gået igennem og du kan hente den på Aalborg Sportshøjskole indtil {date}

	%% TABLE
	\vspace{{20pt}}
	\setlength\doublerulesep{{5pt}}
	\noindent
	\begin{{tabularx}}{{\textwidth}}{{| l | X | c | c | >{{\columncolor{{ASH-LightBlue}}}}c |}}
		\hline\rowcolor{{ASH-Black}}
		\leavevmode\color{{ASH-Yellow}}\raggedleft\textbf{{ID}} & 
		\leavevmode\color{{ASH-Yellow}}\textbf{{Navn}} & 
		\leavevmode\color{{ASH-Yellow}}\textbf{{Antal}} & 
		\leavevmode\color{{ASH-Yellow}}\textbf{{Pris pr. stk. (DKK)}} & 
		\leavevmode\color{{ASH-Yellow}}\textbf{{Samlet pris(DKK)}} \\
	\hline
	\hline
		{table}
		\\\hline
	\end{{tabularx}}
	\par\vspace{{8pt}}
	\begin{{flushright}}
	\noindent
		\begin{{tabular}}{{r r}}
			\textbf{{Total}}      & {total}	\\
			\textbf{{Heraf moms}} & {salestax} \\
		\end{{tabular}}
	\end{{flushright}}
	%% QR Code
	\vspace{{-40pt}}
	\begin{{minipage}}[t]{{0.25\linewidth}}
		\vspace{{0pt}}
		\includegraphics[width=\textwidth]{{qr-code.png}}
	\end{{minipage}}
	\hspace{{5pt}}
	\begin{{minipage}}[t]{{0.25\linewidth}}
		\vspace{{3pt}}
		\textit{{Medbring denne QR-Kode når du henter dine varere.}}\\
		
		\vspace{{60pt}}{{
		\textbf{{Order Id:}}\
		{id}}}
	\end{{minipage}}
	
\end{{document}}
	";
	}
}


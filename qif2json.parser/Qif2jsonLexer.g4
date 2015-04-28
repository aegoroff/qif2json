lexer grammar Qif2jsonLexer;

TYPE_START : 'Type:' ;
ACCOUNT : 'Account' ;

TYPE_MARKER : '!' ;

END_TRANSACTION : '^' ;

TYPE : 'Cash'
	 | 'Bank'
	 | 'CCard'
	 | 'Invst'
	 | 'Oth A'
	 | 'Oth L'
	 | 'Invoice'
	 | 'Cat'
	 | 'Class'
	 | 'Memorized'
	 ;

LINE_START : CODE  -> pushMode(STR) ;

CODE : 'D'
	 | 'T'
	 | 'M'
	 | 'C'
	 | 'N'
	 | 'P'
	 | 'A'
	 | 'L'
	 | 'F'
	 | 'S'
	 | 'E'
	 | '$'
	 | '%'
	 | 'Y'
	 | 'I'
	 | 'Q'
	 | 'O'
	 | 'X'
	 | 'XA'
	 | 'XI'
	 | 'XE'
	 | 'XC'
	 | 'XR'
	 | 'XT'
	 | 'XS'
	 | 'XN'
	 | 'X#'
	 | 'X$'
	 | 'XF'
	 ;

LINE_BREAK : [\r]? [\n] ;

mode STR;

LITERAL : TEXT LINE_BREAK -> popMode ;
fragment TEXT : ~[\r\n]*  ;

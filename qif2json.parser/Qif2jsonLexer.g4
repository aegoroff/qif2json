// Created by: egr
// Created at: 30.04.2015
// QIF file format lexer grammar
// © 2015 Alexander Egorov

lexer grammar Qif2jsonLexer;

TYPE_START : 'Type:' ;
ACCOUNT : 'Account' ;

TYPE_MARKER : '!' ;

END_TRANSACTION : '^' ;

TYPE : 'Cash' // Cash account transactions
	 | 'Bank' // Bank account transactions
	 | 'CCard' // Credit card account transactions
	 | 'Invst' // Investment account transactions
	 | 'Oth A' // Asset account transactions
	 | 'Oth L' // Liability account transactions
	 | 'Invoice'
	 | 'Cat' // Category list
	 | 'Class' // Class list
	 | 'Memorized' // Memorized transaction list
	 ;

LINE_START : CODE  -> pushMode(STR) ;

fragment CODE 
     : 'D'
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
	 | '/'
	 | '1'
	 | '2'
	 | '3'
	 | '4'
	 | '5'
	 | '6'
	 | '7'
	 | 'KC'
	 | 'KD'
	 | 'KP'
	 | 'KI'
	 | 'KE'
	 ;

LINE_BREAK : [\r]? [\n] ;

mode STR;

LITERAL : TEXT LINE_BREAK -> popMode ;
fragment TEXT : ~[\r\n]*  ;

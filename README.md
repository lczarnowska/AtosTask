Zadania utworzone dla firmy ATOS.
Kilka uwag:
1) W opisie zadania był Firstname. Założyłam, że to literówka i poprawiłam nazwę w modelu na FirstName (N z WIELKIEJ litery)
2) Treść zadania sugerowała, aby zwracac bezpośrednio model - dlatego, nie mam DTO, ani Automappera
3) Założyłam, że Imię i Nazwisko muszą mieć co najmniej 1 znak, aby zaprezentować walidację 
4) W przypadku błędu walidacji zwracam kod nie 400, jak standardowo ASP .Net tylko 422 (Unprocessable Entity), więc jeśli API będzie testowane automatem - to proszę zwrócić na to uwagę. Jest to opisane w RFC i widać trend, że coraz częsciej jest stosowane https://httpwg.org/specs/rfc9110.html#status.422
5) Z racji tego, że zadanie sugerowało zastosowanie mocka dla warstwy persystencji - warstwa ta jest zamockowana, a co za tym idzie nie wykonywałam testów jednostkowych dla mocka. Co więcej, warstwa persystencji jest oparta o Listę, a nie o kolekcję odporną na równoległy zapis (bo po co w mocku), więc zdaję sobie sprawę, że API jest wrażliwe na równoległy zapis  i odczyt. W tej sytuacji poleci błąd 500.
6) Konfiguracja 'ReturnHttpNotAcceptable = true' wymusza w Swagger ustawienie 'Media type = application/json' lub 'Media type = application/xml' w przeciwnym wypadku zwracany jest kod 406
7) Ze względu na mała liczbę danych nie ma zastosowanego ResponseCaching
8) Delete nie jest idempotentny, bo nie mam statusu na usunięty. Wielokrotny DELETE zwraca NotFound przy kolejnych odwołaniach.
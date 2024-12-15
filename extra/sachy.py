class Sachovnice:
    def __init__(self):
        self.sachovnice = self.vytvorit_sachovnici()
        self.hrac_na_tahu = "bile"

    def vytvorit_sachovnici(self):
        bile = ["♖", "♘", "♗", "♕", "♔", "♗", "♘", "♖"]
        bile_pesi = ["♙"] * 8
        cerne = ["♜", "♞", "♝", "♛", "♚", "♝", "♞", "♜"]
        cerne_pesi = ["♟"] * 8
        prazdne = ["⬜" if (i + j) % 2 == 0 else "⬛" for i in range(8) for j in range(8)]

        return [
            cerne,
            cerne_pesi,
            prazdne[:8],
            prazdne[8:16],
            prazdne[16:24],
            prazdne[24:32],
            bile_pesi,
            bile,
        ]

    def vykreslit(self):
        for radek in self.sachovnice:
            print(" ".join(radek))
        print("\n")

    def pozice_na_index(self, pozice):
        sloupec = ord(pozice[0].lower()) - ord("a")
        radek = 8 - int(pozice[1])
        return radek, sloupec

    def index_na_pozici(self, radek, sloupec):
        pismeno = chr(sloupec + ord("a"))
        cislo = str(8 - radek)
        return f"{pismeno}{cislo}"

    def je_legalni_tah(self, start, konec):
        radek_start, sloupec_start = self.pozice_na_index(start)
        radek_konec, sloupec_konec = self.pozice_na_index(konec)

        figurka = self.sachovnice[radek_start][sloupec_start]
        cil = self.sachovnice[radek_konec][sloupec_konec]

        if not self.je_tah_hrace(figurka):
            print("Hrajete s figurkami protivníka.")
            return False

        if not self.je_pohyb_legalni(figurka, radek_start, sloupec_start, radek_konec, sloupec_konec):
            print("Neplatný tah pro tuto figurku.")
            return False

        return True

    def je_tah_hrace(self, figurka):
        if self.hrac_na_tahu == "bile" and figurka in "♙♖♘♗♕♔":
            return True
        if self.hrac_na_tahu == "cerne" and figurka in "♟♜♞♝♛♚":
            return True
        return False

    def je_pohyb_legalni(self, figurka, radek_start, sloupec_start, radek_konec, sloupec_konec):
        if figurka in "♙♟":
            return self.tah_pesce(figurka, radek_start, sloupec_start, radek_konec, sloupec_konec)
        elif figurka in "♖♜":
            return self.tah_veze(radek_start, sloupec_start, radek_konec, sloupec_konec)
        elif figurka in "♘♞":
            return self.tah_jezdece(radek_start, sloupec_start, radek_konec, sloupec_konec)
        elif figurka in "♗♝":
            return self.tah_strelce(radek_start, sloupec_start, radek_konec, sloupec_konec)
        elif figurka in "♕♛":
            return self.tah_damy(radek_start, sloupec_start, radek_konec, sloupec_konec)
        elif figurka in "♔♚":
            return self.tah_krale(radek_start, sloupec_start, radek_konec, sloupec_konec)
        return False

    def tah_pesce(self, figurka, radek_start, sloupec_start, radek_konec, sloupec_konec):
        smer = -1 if figurka == "♙" else 1
        if sloupec_start == sloupec_konec:
            if radek_konec == radek_start + smer and self.je_prazdne(radek_konec, sloupec_konec):
                return True
        return False

    def tah_veze(self, radek_start, sloupec_start, radek_konec, sloupec_konec):
        if radek_start == radek_konec or sloupec_start == sloupec_konec:
            return self.je_cesta_volna(radek_start, sloupec_start, radek_konec, sloupec_konec)
        return False

    def tah_jezdece(self, radek_start, sloupec_start, radek_konec, sloupec_konec):
        return (abs(radek_start - radek_konec), abs(sloupec_start - sloupec_konec)) in [(2, 1), (1, 2)]

    def tah_strelce(self, radek_start, sloupec_start, radek_konec, sloupec_konec):
        if abs(radek_start - radek_konec) == abs(sloupec_start - sloupec_konec):
            return self.je_cesta_volna(radek_start, sloupec_start, radek_konec, sloupec_konec)
        return False

    def tah_damy(self, radek_start, sloupec_start, radek_konec, sloupec_konec):
        return self.tah_veze(radek_start, sloupec_start, radek_konec, sloupec_konec) or self.tah_strelce(
            radek_start, sloupec_start, radek_konec, sloupec_konec
        )

    def tah_krale(self, radek_start, sloupec_start, radek_konec, sloupec_konec):
        return abs(radek_start - radek_konec) <= 1 and abs(sloupec_start - sloupec_konec) <= 1

    def je_prazdne(self, radek, sloupec):
        obsah = self.sachovnice[radek][sloupec]
        return obsah == "⬜" or obsah == "⬛"

    def je_cesta_volna(self, radek_start, sloupec_start, radek_konec, sloupec_konec):
        if radek_start == radek_konec:
            krok = 1 if sloupec_start < sloupec_konec else -1
            for sloupec in range(sloupec_start + krok, sloupec_konec, krok):
                if not self.je_prazdne(radek_start, sloupec):
                    return False
        elif sloupec_start == sloupec_konec:
            krok = 1 if radek_start < radek_konec else -1
            for radek in range(radek_start + krok, radek_konec, krok):
                if not self.je_prazdne(radek, sloupec_start):
                    return False
        return True


def spustit_hru():
    hra = Sachovnice()
    hra.vykreslit()

    while True:
        print(f"Hraje {hra.hrac_na_tahu}.")
        start = input("Zadejte počáteční pozici (např. e2): ")
        konec = input("Zadejte cílovou pozici (např. e4): ")

        if hra.je_legalni_tah(start, konec):
            radek_start, sloupec_start = hra.pozice_na_index(start)
            radek_konec, sloupec_konec = hra.pozice_na_index(konec)

            hra.sachovnice[radek_konec][sloupec_konec] = hra.sachovnice[radek_start][sloupec_start]
            hra.sachovnice[radek_start][sloupec_start] = "⬜" if (radek_start + sloupec_start) % 2 == 0 else "⬛"

            hra.hrac_na_tahu = "cerne" if hra.hrac_na_tahu == "bile" else "bile"
            hra.vykreslit()
        else:
            print("Tah není platný, zkuste to znovu.")


spustit_hru()
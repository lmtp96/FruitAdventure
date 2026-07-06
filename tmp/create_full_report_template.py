from pathlib import Path

from docx import Document
from docx.enum.section import WD_SECTION
from docx.enum.table import WD_CELL_VERTICAL_ALIGNMENT, WD_TABLE_ALIGNMENT
from docx.enum.text import WD_ALIGN_PARAGRAPH
from docx.oxml import OxmlElement
from docx.oxml.ns import qn
from docx.shared import Cm, Pt, RGBColor


OUT = Path(r"D:\Game Project\FruitAdventure\output\documents\Mau_Khung_Bao_Cao_Fruit_Adventure.docx")

FONT = "Times New Roman"
BLACK = RGBColor(0, 0, 0)
MUTED = RGBColor(90, 90, 90)
LIGHT_GRAY = "F2F2F2"


def set_run_font(run, size=None, bold=None, italic=None, color=None):
    run.font.name = FONT
    run._element.rPr.rFonts.set(qn("w:ascii"), FONT)
    run._element.rPr.rFonts.set(qn("w:hAnsi"), FONT)
    run._element.rPr.rFonts.set(qn("w:eastAsia"), FONT)
    if size is not None:
        run.font.size = Pt(size)
    if bold is not None:
        run.bold = bold
    if italic is not None:
        run.italic = italic
    if color is not None:
        run.font.color.rgb = color


def set_cell_text(cell, text, bold=False, size=12, align=WD_ALIGN_PARAGRAPH.LEFT, fill=None):
    cell.vertical_alignment = WD_CELL_VERTICAL_ALIGNMENT.CENTER
    if fill:
        tc_pr = cell._tc.get_or_add_tcPr()
        shd = OxmlElement("w:shd")
        shd.set(qn("w:fill"), fill)
        tc_pr.append(shd)
    p = cell.paragraphs[0]
    p.alignment = align
    p.paragraph_format.space_before = Pt(0)
    p.paragraph_format.space_after = Pt(0)
    run = p.add_run(text)
    set_run_font(run, size=size, bold=bold)


def set_table_borders(table, color="000000", size="4"):
    tbl_pr = table._tbl.tblPr
    borders = tbl_pr.first_child_found_in("w:tblBorders")
    if borders is None:
        borders = OxmlElement("w:tblBorders")
        tbl_pr.append(borders)
    for edge in ("top", "left", "bottom", "right", "insideH", "insideV"):
        element = borders.find(qn(f"w:{edge}"))
        if element is None:
            element = OxmlElement(f"w:{edge}")
            borders.append(element)
        element.set(qn("w:val"), "single")
        element.set(qn("w:sz"), size)
        element.set(qn("w:space"), "0")
        element.set(qn("w:color"), color)


def set_table_width(table, widths_cm):
    table.autofit = False
    for row in table.rows:
        for idx, width in enumerate(widths_cm):
            cell = row.cells[idx]
            cell.width = Cm(width)
            tc_pr = cell._tc.get_or_add_tcPr()
            tc_w = tc_pr.first_child_found_in("w:tcW")
            if tc_w is None:
                tc_w = OxmlElement("w:tcW")
                tc_pr.append(tc_w)
            tc_w.set(qn("w:w"), str(int(Cm(width).twips)))
            tc_w.set(qn("w:type"), "dxa")


def paragraph(text="", size=13, bold=False, italic=False, align=WD_ALIGN_PARAGRAPH.LEFT, before=0, after=6, style=None, color=BLACK):
    p = doc.add_paragraph(style=style)
    p.alignment = align
    p.paragraph_format.space_before = Pt(before)
    p.paragraph_format.space_after = Pt(after)
    p.paragraph_format.line_spacing = 1.3
    if text:
        run = p.add_run(text)
        set_run_font(run, size=size, bold=bold, italic=italic, color=color)
    return p


def centered_title(text, size=16, before=0, after=12):
    return paragraph(text, size=size, bold=True, align=WD_ALIGN_PARAGRAPH.CENTER, before=before, after=after)


def add_blank_lines(count=8):
    for _ in range(count):
        paragraph(".................................................................................................................................", size=13, after=4)


def add_cover_border(section):
    sect_pr = section._sectPr
    pg_borders = OxmlElement("w:pgBorders")
    pg_borders.set(qn("w:offsetFrom"), "page")
    for edge in ("top", "left", "bottom", "right"):
        border = OxmlElement(f"w:{edge}")
        border.set(qn("w:val"), "double")
        border.set(qn("w:sz"), "12")
        border.set(qn("w:space"), "24")
        border.set(qn("w:color"), "000000")
        pg_borders.append(border)
    sect_pr.append(pg_borders)


def add_page_number_footer(section):
    footer = section.footer
    p = footer.paragraphs[0]
    p.alignment = WD_ALIGN_PARAGRAPH.CENTER
    run = p.add_run()
    fld_begin = OxmlElement("w:fldChar")
    fld_begin.set(qn("w:fldCharType"), "begin")
    instr = OxmlElement("w:instrText")
    instr.set(qn("xml:space"), "preserve")
    instr.text = "PAGE"
    fld_end = OxmlElement("w:fldChar")
    fld_end.set(qn("w:fldCharType"), "end")
    run._r.append(fld_begin)
    run._r.append(instr)
    run._r.append(fld_end)
    set_run_font(run, size=12)


def add_toc_field():
    p = doc.add_paragraph()
    p.paragraph_format.space_after = Pt(8)
    run = p.add_run()
    fld_begin = OxmlElement("w:fldChar")
    fld_begin.set(qn("w:fldCharType"), "begin")
    instr = OxmlElement("w:instrText")
    instr.set(qn("xml:space"), "preserve")
    instr.text = r'TOC \o "1-3" \h \z \u'
    fld_sep = OxmlElement("w:fldChar")
    fld_sep.set(qn("w:fldCharType"), "separate")
    text = OxmlElement("w:t")
    text.text = "Bấm chuột phải vào đây và chọn Update Field để cập nhật mục lục tự động."
    fld_end = OxmlElement("w:fldChar")
    fld_end.set(qn("w:fldCharType"), "end")
    run._r.append(fld_begin)
    run._r.append(instr)
    run._r.append(fld_sep)
    run._r.append(text)
    run._r.append(fld_end)
    set_run_font(run, size=13, italic=True, color=MUTED)


def add_placeholder_box(label, height_lines=6):
    table = doc.add_table(rows=1, cols=1)
    table.alignment = WD_TABLE_ALIGNMENT.CENTER
    set_table_borders(table, color="A6A6A6", size="4")
    cell = table.cell(0, 0)
    set_cell_text(cell, label + "\n\n" + "\n".join(["" for _ in range(height_lines)]), size=12, align=WD_ALIGN_PARAGRAPH.CENTER)
    paragraph("", after=3)


def add_heading(text, level=1):
    p = doc.add_heading(text, level=level)
    p.alignment = WD_ALIGN_PARAGRAPH.LEFT
    for run in p.runs:
        set_run_font(run, size={1: 16, 2: 14, 3: 13}.get(level, 13), bold=True, color=BLACK)
    return p


def add_note(text):
    p = paragraph("[Gợi ý viết] " + text, size=12, italic=True, color=MUTED, after=8)
    return p


def add_bullets(items):
    for item in items:
        p = doc.add_paragraph(style="List Bullet")
        p.paragraph_format.space_after = Pt(4)
        p.paragraph_format.line_spacing = 1.25
        run = p.add_run(item)
        set_run_font(run, size=13)


def add_review_page(title, subtitle=None):
    centered_title(title, size=16, after=10)
    if subtitle:
        centered_title(subtitle, size=13, after=14)
    paragraph("1. Họ và tên học viên được giao đề tài", bold=True)
    paragraph("Học viên: [Tên học viên]        MSHV: [Mã số học viên]        Lớp: [Tên lớp]")
    paragraph("Ngành: Công nghệ thông tin")
    paragraph("2. Tên đề tài: Lập trình game 2D Platformer Fruit Adventure.", bold=True)
    paragraph("3. Tổng quát về đồ án:", bold=True)
    paragraph("Số trang: [Điền sau]        Số chương: 11")
    paragraph("4. Nhận xét", bold=True)
    paragraph("i. Về tinh thần, thái độ làm việc của học viên")
    add_blank_lines(3)
    paragraph("ii. Những kết quả đạt được của đề tài")
    add_blank_lines(3)
    paragraph("iii. Những hạn chế của đề tài")
    add_blank_lines(3)
    paragraph("5. Đề nghị        ☐ Được bảo vệ        ☐ Không được bảo vệ", bold=True)
    paragraph("TP. Hồ Chí Minh, ngày ...... tháng ...... năm 2026", align=WD_ALIGN_PARAGRAPH.RIGHT)
    paragraph("Giảng viên/Hội đồng", align=WD_ALIGN_PARAGRAPH.RIGHT, bold=True)
    paragraph("(Ký và ghi rõ họ tên)", align=WD_ALIGN_PARAGRAPH.RIGHT, italic=True)


def add_front_matter_page(title, body):
    centered_title(title, size=16, after=18)
    for text in body:
        paragraph(text, align=WD_ALIGN_PARAGRAPH.JUSTIFY, after=8)


def add_overview_page():
    centered_title("TỔNG QUAN ĐỒ ÁN", size=16, after=14)
    rows = [
        ("Tên đề tài", "Lập trình game 2D Platformer Fruit Adventure"),
        ("Giảng viên hướng dẫn", "[Tên giảng viên]"),
        ("Thời gian thực hiện", "[Tháng/Năm] đến [Tháng/Năm]"),
        ("Học viên thực hiện", "[Tên học viên]"),
        ("Nội dung đề tài", "Xây dựng game 2D platformer bằng Unity, người chơi vượt màn, thu thập trái cây, né bẫy, tiêu diệt kẻ địch và hoàn thành các cấp độ."),
        ("Mục tiêu", "Game chạy được trên PC/WebGL hoặc nền tảng mục tiêu; có menu, chọn level, chọn skin, độ khó, gameplay hoàn chỉnh và lưu tiến trình."),
        ("Phạm vi đối tượng", "Người chơi yêu thích game platformer 2D, thao tác đơn giản, độ tuổi tham khảo 10-25."),
        ("Phương pháp thực hiện", "Lập trình C# trên Unity, sử dụng hệ thống 2D, Tilemap, Input System, Cinemachine, UI và PlayerPrefs."),
        ("Kết quả mong đợi", "Hoàn thiện một bản game có thể chơi được, có trải nghiệm rõ ràng, giao diện dễ hiểu và âm thanh/hiệu ứng đầy đủ."),
    ]
    table = doc.add_table(rows=len(rows), cols=2)
    table.alignment = WD_TABLE_ALIGNMENT.CENTER
    set_table_borders(table)
    set_table_width(table, [4.5, 11.5])
    for row, (label, value) in zip(table.rows, rows):
        set_cell_text(row.cells[0], label, bold=True, size=12, fill=LIGHT_GRAY)
        set_cell_text(row.cells[1], value, size=12)


def add_chapter(title, sections):
    doc.add_page_break()
    add_heading(title, level=1)
    for heading, note, bullets in sections:
        add_heading(heading, level=2)
        add_note(note)
        if bullets:
            add_bullets(bullets)


doc = Document()
section = doc.sections[0]
section.page_width = Cm(21.0)
section.page_height = Cm(29.7)
section.top_margin = Cm(2.0)
section.bottom_margin = Cm(2.0)
section.left_margin = Cm(2.5)
section.right_margin = Cm(2.0)
section.header_distance = Cm(1.2)
section.footer_distance = Cm(1.2)
add_cover_border(section)

styles = doc.styles
normal = styles["Normal"]
normal.font.name = FONT
normal._element.rPr.rFonts.set(qn("w:ascii"), FONT)
normal._element.rPr.rFonts.set(qn("w:hAnsi"), FONT)
normal._element.rPr.rFonts.set(qn("w:eastAsia"), FONT)
normal.font.size = Pt(13)
normal.paragraph_format.space_after = Pt(6)
normal.paragraph_format.line_spacing = 1.3

for style_name, size in (("Heading 1", 16), ("Heading 2", 14), ("Heading 3", 13)):
    style = styles[style_name]
    style.font.name = FONT
    style._element.rPr.rFonts.set(qn("w:ascii"), FONT)
    style._element.rPr.rFonts.set(qn("w:hAnsi"), FONT)
    style._element.rPr.rFonts.set(qn("w:eastAsia"), FONT)
    style.font.size = Pt(size)
    style.font.bold = True
    style.font.color.rgb = BLACK
    style.paragraph_format.space_before = Pt(12)
    style.paragraph_format.space_after = Pt(6)
    style.paragraph_format.line_spacing = 1.3

styles["List Bullet"].font.name = FONT
styles["List Bullet"].font.size = Pt(13)

# Cover page
centered_title("HỌC VIỆN CÔNG NGHỆ THÔNG TIN & THIẾT KẾ VTC", size=16, after=0)
centered_title("ACADEMY", size=16, after=45)
add_placeholder_box("[CHÈN LOGO HỌC VIỆN TẠI ĐÂY]", height_lines=2)
centered_title("CAPSTONE PROJECT", size=18, before=28, after=38)
centered_title("LẬP TRÌNH GAME 2D PLATFORMER", size=20, after=3)
centered_title("FRUIT ADVENTURE", size=22, after=26)

for label, value in [
    ("Ngành:", "CÔNG NGHỆ THÔNG TIN"),
    ("Chuyên ngành:", "LẬP TRÌNH GAME"),
    ("Giảng viên hướng dẫn:", "[Tên giảng viên]"),
    ("Học viên thực hiện:", "[Tên học viên]"),
    ("MSHV:", "[Mã số học viên]        Lớp: [Tên lớp]"),
]:
    p = paragraph("", after=5)
    p.paragraph_format.left_indent = Cm(3.2)
    p.paragraph_format.tab_stops.add_tab_stop(Cm(8.4))
    r1 = p.add_run(label)
    set_run_font(r1, size=14)
    r2 = p.add_run(f"\t{value}")
    set_run_font(r2, size=14, bold=True)

centered_title("TP. Hồ Chí Minh, 2026", size=15, before=45, after=0)

# New section without cover border.
doc.add_section(WD_SECTION.NEW_PAGE)
body_section = doc.sections[-1]
body_section.page_width = Cm(21.0)
body_section.page_height = Cm(29.7)
body_section.top_margin = Cm(2.0)
body_section.bottom_margin = Cm(2.0)
body_section.left_margin = Cm(2.5)
body_section.right_margin = Cm(2.0)
body_section.header_distance = Cm(1.2)
body_section.footer_distance = Cm(1.2)
add_page_number_footer(body_section)

add_review_page("BẢN NHẬN XÉT CỦA GIẢNG VIÊN HƯỚNG DẪN", "CAPSTONE PROJECT")
doc.add_page_break()
add_review_page("NHẬN XÉT CỦA GIẢNG VIÊN PHẢN BIỆN")
doc.add_page_break()

add_front_matter_page("LỜI CẢM ƠN", [
    "Nhóm chúng em chân thành cảm ơn quý thầy cô tại VTC Academy đã hỗ trợ, hướng dẫn và truyền đạt kiến thức trong quá trình học tập và thực hiện đồ án.",
    "Chúng em xin cảm ơn giảng viên hướng dẫn đã góp ý, định hướng và hỗ trợ nhóm hoàn thiện sản phẩm game Fruit Adventure cũng như báo cáo đồ án.",
    "Trong quá trình thực hiện, sản phẩm và báo cáo có thể còn thiếu sót. Chúng em kính mong nhận được góp ý từ thầy cô để tiếp tục hoàn thiện tốt hơn.",
    "TP. Hồ Chí Minh, tháng ...... năm 2026\nHọc viên thực hiện\n[Tên học viên]",
])
doc.add_page_break()

add_front_matter_page("LỜI CAM KẾT", [
    "Em/chúng em cam kết sản phẩm game Fruit Adventure và báo cáo này được thực hiện trong quá trình học tập, nghiên cứu và phát triển đồ án.",
    "Các tài nguyên, hình ảnh, âm thanh và công cụ sử dụng trong game sẽ được liệt kê ở phần tài liệu tham khảo hoặc ghi nguồn phù hợp nếu có sử dụng từ bên thứ ba.",
    "Em/chúng em chịu trách nhiệm về nội dung trình bày trong báo cáo và sản phẩm nộp cho môn Capstone Project.",
    "TP. Hồ Chí Minh, tháng ...... năm 2026\nHọc viên thực hiện\n[Tên học viên]",
])
doc.add_page_break()

add_overview_page()
doc.add_page_break()

centered_title("MỤC LỤC", size=16, after=14)
add_toc_field()
paragraph("Lưu ý: Sau khi viết xong, bấm Ctrl + A rồi F9 trong Word để cập nhật mục lục, số trang, danh mục hình/bảng.", italic=True, color=MUTED)
doc.add_page_break()

centered_title("DANH MỤC CÁC BẢNG", size=16, after=12)
paragraph("Bảng 7.1: Danh sách cấp độ trong game ........................................................ [Trang]")
paragraph("Bảng 9.1: Danh sách lớp đối tượng .................................................................. [Trang]")
paragraph("Bảng 10.1: Kế hoạch phát triển game ............................................................. [Trang]")
paragraph("Bảng 10.2: Bảng kiểm thử chất lượng ............................................................ [Trang]")
doc.add_page_break()

centered_title("DANH MỤC CÁC HÌNH", size=16, after=12)
paragraph("Hình 2.1: Màn hình Main Menu ........................................................................ [Trang]")
paragraph("Hình 2.2: Màn hình gameplay Level 1 ............................................................. [Trang]")
paragraph("Hình 6.1: Nhân vật chính .................................................................................. [Trang]")
paragraph("Hình 7.1: Bản đồ Level 1 .................................................................................. [Trang]")
paragraph("Hình 9.1: Use Case Diagram ............................................................................ [Trang]")

chapters = [
    ("CHƯƠNG 1: CƠ SỞ LÝ THUYẾT", [
        ("1.1 Tổng quan đề tài", "Nêu lý do chọn game 2D platformer Fruit Adventure và vấn đề game giải quyết trong phạm vi đồ án.", ["Đặt vấn đề và giải pháp", "Nội dung đề tài", "Mục tiêu đề tài", "Phạm vi đề tài"]),
        ("1.2 Công cụ sử dụng", "Liệt kê công cụ, engine, ngôn ngữ và package chính đang dùng trong project.", ["Unity 6000.3.9f1", "C#", "URP 2D", "Input System", "Cinemachine", "Tilemap", "TextMeshPro/UGUI"]),
    ]),
    ("CHƯƠNG 2: TỔNG QUAN VỀ GAME", [
        ("2.1 Nội dung và mục tiêu của game", "Tóm tắt Fruit Adventure: người chơi vượt màn, thu thập trái cây, né bẫy và hoàn thành level.", ["Đối tượng người chơi hướng đến", "Mục tiêu gameplay", "Điểm nổi bật của game"]),
        ("2.2 Thể loại của game", "Trình bày game thuộc thể loại 2D platformer, adventure/casual.", []),
        ("2.3 Cấu trúc game", "Mô tả MainMenu, Level_1, Level_2, Level_3, TheEnd và luồng chơi từ đầu đến cuối.", []),
    ]),
    ("CHƯƠNG 3: CÁCH CHƠI, MỤC TIÊU VÀ TIẾN TRIỂN TRONG GAME", [
        ("3.1 Mục tiêu chính và mục tiêu phụ", "Viết mục tiêu qua màn, thu thập trái cây, tránh mất mạng, đạt thời gian tốt.", []),
        ("3.2 Cơ chế điều khiển", "Mô tả di chuyển trái/phải, nhảy, double jump, wall jump, checkpoint.", []),
        ("3.3 Cân bằng độ khó", "So sánh Easy, Normal, Hard và cách game tăng độ khó qua từng màn.", []),
    ]),
    ("CHƯƠNG 4: QUY TẮC VÀ CƠ CHẾ VẬN HÀNH GAME", [
        ("4.1 Quy tắc cơ bản", "Nêu luật va chạm, thu thập trái cây, hoàn thành level, restart khi chết.", []),
        ("4.2 Đối tượng tương tác", "Liệt kê player, fruit, enemy, trap, checkpoint, finish point, UI button.", []),
        ("4.3 Cơ chế lưu tiến trình", "Mô tả PlayerPrefs: mở khóa level, lưu best time, tổng số trái cây, skin cuối cùng.", []),
    ]),
    ("CHƯƠNG 5: HỆ THỐNG ĐỒ HỌA VÀ ÂM THANH", [
        ("5.1 Phong cách đồ họa", "Trình bày sprite 2D, tilemap, background nhiều lớp, hiệu ứng nhặt fruit và player death.", []),
        ("5.2 Góc nhìn và camera", "Mô tả góc nhìn ngang của platformer và camera theo dõi nhân vật.", []),
        ("5.3 Hệ thống âm thanh", "Liệt kê BGM và SFX: jump, wall jump, pickup, finish, death, respawn, menu.", []),
    ]),
    ("CHƯƠNG 6: CỐT TRUYỆN VÀ NHÂN VẬT", [
        ("6.1 Câu chuyện nền", "Viết ngắn về hành trình của nhân vật trong thế giới Fruit Adventure.", []),
        ("6.2 Nhân vật chính", "Mô tả player, skin và animation chính.", []),
        ("6.3 Kẻ địch và vật cản", "Mô tả các enemy như Bat, Bee, BlueBird, Chicken, Ghost, Mushroom, Plant, Rino, Snail, Trunk và các trap.", []),
    ]),
    ("CHƯƠNG 7: CHI TIẾT VỀ THẾ GIỚI GAME VÀ CẤP ĐỘ", [
        ("7.1 Giao diện và cảm giác thế giới game", "Nêu cảm giác game hướng tới: vui nhộn, dễ chơi, thử thách vừa phải.", []),
        ("7.2 Các cấp độ trong game", "Tạo bảng mô tả Level_1, Level_2, Level_3: mục tiêu, enemy, trap, checkpoint, độ khó.", []),
        ("7.3 Hình ảnh minh họa", "Chèn ảnh chụp MainMenu, Level Selection, Level 1-3, TheEnd.", []),
    ]),
    ("CHƯƠNG 8: PHÂN TÍCH, THIẾT KẾ HỆ THỐNG", [
        ("8.1 Yêu cầu chức năng", "Liệt kê chức năng: chơi mới, tiếp tục, chọn level, chọn skin, chọn độ khó, chơi level, lưu tiến trình.", []),
        ("8.2 Yêu cầu phi chức năng", "Nêu yêu cầu về hiệu năng, thao tác mượt, UI dễ hiểu, âm thanh rõ, dễ mở rộng level.", []),
    ]),
    ("CHƯƠNG 9: PHÂN TÍCH, THIẾT KẾ HỆ THỐNG HƯỚNG ĐỐI TƯỢNG", [
        ("9.1 Use Case Diagram", "Vẽ sơ đồ người chơi tương tác với menu, gameplay, chọn level, chọn skin, settings.", []),
        ("9.2 Activity Diagram", "Vẽ luồng bắt đầu game, nhặt fruit, chết/respawn, hoàn thành level.", []),
        ("9.3 Sequence Diagram", "Vẽ luồng Player chạm Fruit, Player chạm FinishPoint, Player bị DamageTrigger.", []),
        ("9.4 Class Diagram", "Đưa các lớp Player, GameManager, PlayerManager, AudioManager, SkinManager, DifficultyManager, Fruit, Enemy, Trap, UI.", []),
    ]),
    ("CHƯƠNG 10: TỐI ƯU HÓA, KẾ HOẠCH PHÁT TRIỂN, KIỂM THỬ VÀ PHÁT HÀNH", [
        ("10.1 Tối ưu hóa", "Nêu cách tối ưu sprite, prefab, scene, audio, object creation và tránh lỗi logic.", []),
        ("10.2 Kế hoạch phát triển", "Tạo bảng công việc theo tuần: thiết kế map, code player, enemy, trap, UI, audio, test.", []),
        ("10.3 Kiểm thử chất lượng", "Tạo bảng test case cho movement, fruit, checkpoint, finish, enemy, trap, save/load.", []),
        ("10.4 Phát hành game", "Nêu nền tảng dự kiến: PC/WebGL/Android nếu có build.", []),
    ]),
    ("CHƯƠNG 11: KẾT LUẬN VÀ HƯỚNG PHÁT TRIỂN", [
        ("11.1 Kết luận", "Tóm tắt kết quả đã đạt được so với mục tiêu ban đầu.", []),
        ("11.2 Ưu điểm", "Nêu điểm mạnh: gameplay rõ, nhiều enemy/trap, checkpoint, level progression, skin, audio.", []),
        ("11.3 Hạn chế", "Nêu hạn chế hiện tại: số level còn ít, chưa có boss/leaderboard/mobile tối ưu nếu chưa làm.", []),
        ("11.4 Hướng phát triển", "Đề xuất thêm level, boss, nhiệm vụ, nâng cấp nhân vật, bảng xếp hạng, tối ưu build.", []),
    ]),
]

for chapter_title, sections in chapters:
    add_chapter(chapter_title, sections)

doc.add_page_break()
add_heading("TÀI LIỆU THAM KHẢO", level=1)
add_note("Liệt kê tài liệu Unity, asset, âm thanh, hình ảnh, tutorial hoặc nguồn tham khảo đã dùng.")
paragraph("[1] Unity Documentation - https://docs.unity3d.com/")
paragraph("[2] Nguồn asset/hình ảnh/âm thanh sử dụng trong game: [Điền nguồn]")

OUT.parent.mkdir(parents=True, exist_ok=True)
doc.save(OUT)
print(OUT)

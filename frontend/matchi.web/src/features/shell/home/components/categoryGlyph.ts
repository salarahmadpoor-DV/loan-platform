import HomeOutlined from "@mui/icons-material/HomeOutlined";
import DevicesOutlined from "@mui/icons-material/DevicesOutlined";
import BusinessOutlined from "@mui/icons-material/BusinessOutlined";
import BrushOutlined from "@mui/icons-material/BrushOutlined";
import CampaignOutlined from "@mui/icons-material/CampaignOutlined";
import GavelOutlined from "@mui/icons-material/GavelOutlined";
import CalculateOutlined from "@mui/icons-material/CalculateOutlined";
import SchoolOutlined from "@mui/icons-material/SchoolOutlined";
import DirectionsCarOutlined from "@mui/icons-material/DirectionsCarOutlined";
import FavoriteBorderOutlined from "@mui/icons-material/FavoriteBorderOutlined";
import CategoryOutlined from "@mui/icons-material/CategoryOutlined";

type CategoryGlyph = typeof HomeOutlined;

const BY_ID: Record<string, CategoryGlyph> = {
  home: HomeOutlined,
  tech: DevicesOutlined,
  business: BusinessOutlined,
  design: BrushOutlined,
  marketing: CampaignOutlined,
  legal: GavelOutlined,
  accounting: CalculateOutlined,
  education: SchoolOutlined,
  auto: DirectionsCarOutlined,
  health: FavoriteBorderOutlined,
};

const TITLE_HINTS: [RegExp, CategoryGlyph][] = [
  [/home|خان/i, HomeOutlined],
  [/tech|فناور|کامپیوتر|it\b/i, DevicesOutlined],
  [/business|کسب/i, BusinessOutlined],
  [/design|طراح/i, BrushOutlined],
  [/market|بازاریاب/i, CampaignOutlined],
  [/legal|حقوق/i, GavelOutlined],
  [/account|حساب/i, CalculateOutlined],
  [/educat|آموزش/i, SchoolOutlined],
  [/auto|خودرو|car/i, DirectionsCarOutlined],
  [/health|سلامت|پزشک/i, FavoriteBorderOutlined],
];

/** Outlined category glyph for discovery cards. No remote images. */
export function categoryGlyph(id: string, title: string): CategoryGlyph {
  if (BY_ID[id]) {
    return BY_ID[id];
  }
  for (const [pattern, Icon] of TITLE_HINTS) {
    if (pattern.test(title) || pattern.test(id)) {
      return Icon;
    }
  }
  return CategoryOutlined;
}

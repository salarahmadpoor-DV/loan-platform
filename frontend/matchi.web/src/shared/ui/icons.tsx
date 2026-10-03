import Add from "@mui/icons-material/Add";
import ArrowBack from "@mui/icons-material/ArrowBack";
import ArrowForward from "@mui/icons-material/ArrowForward";
import AssignmentOutlined from "@mui/icons-material/AssignmentOutlined";
import CheckOutlined from "@mui/icons-material/CheckOutlined";
import DashboardOutlined from "@mui/icons-material/DashboardOutlined";
import ExpandLess from "@mui/icons-material/ExpandLess";
import ExpandMore from "@mui/icons-material/ExpandMore";
import HandshakeOutlined from "@mui/icons-material/HandshakeOutlined";
import InfoOutlined from "@mui/icons-material/InfoOutlined";
import Menu from "@mui/icons-material/Menu";
import MoreHoriz from "@mui/icons-material/MoreHoriz";
import NotificationsNoneOutlined from "@mui/icons-material/NotificationsNoneOutlined";
import PeopleOutline from "@mui/icons-material/PeopleOutline";
import PersonOutline from "@mui/icons-material/PersonOutline";
import RateReviewOutlined from "@mui/icons-material/RateReviewOutlined";
import SearchOutlined from "@mui/icons-material/SearchOutlined";
import SettingsOutlined from "@mui/icons-material/SettingsOutlined";
import StorefrontOutlined from "@mui/icons-material/StorefrontOutlined";
import type { SvgIconProps } from "@mui/material/SvgIcon";
import { useTheme } from "@mui/material/styles";

/** Shared size for chrome icons (header, bottom nav). */
export const chromeIconFontSize = 22;

export const navIcons = {
  dashboard: DashboardOutlined,
  requests: AssignmentOutlined,
  deals: HandshakeOutlined,
  reviews: RateReviewOutlined,
  marketplace: StorefrontOutlined,
  profile: PersonOutline,
  info: InfoOutlined,
  team: PeopleOutline,
  more: MoreHoriz,
  search: SearchOutlined,
  settings: SettingsOutlined,
} as const;

export type NavIconName = keyof typeof navIcons;

export function NavIcon({ name, sx, ...props }: { name: NavIconName } & SvgIconProps) {
  const Icon = navIcons[name];
  return <Icon sx={{ fontSize: chromeIconFontSize, ...sx }} {...props} />;
}

/** Back: toward inline-start (left in LTR, right in RTL). */
export function BackIcon(props: SvgIconProps) {
  const theme = useTheme();
  const { sx, ...rest } = props;
  return (
    <ArrowBack
      sx={{
        fontSize: chromeIconFontSize,
        transform: theme.direction === "rtl" ? "scaleX(-1)" : undefined,
        ...sx,
      }}
      {...rest}
    />
  );
}

/** Forward / “open row”: toward inline-end (right in LTR, left in RTL). */
export function ForwardIcon(props: SvgIconProps) {
  const theme = useTheme();
  const { sx, ...rest } = props;
  return (
    <ArrowForward
      sx={{
        fontSize: chromeIconFontSize,
        transform: theme.direction === "rtl" ? "scaleX(-1)" : undefined,
        ...sx,
      }}
      {...rest}
    />
  );
}

export { Add, CheckOutlined, ExpandLess, ExpandMore, Menu, MoreHoriz, NotificationsNoneOutlined, PersonOutline, SearchOutlined };

import Select from "@mui/material/Select";
import InputLabel from "@mui/material/InputLabel";
import MenuItem from "@mui/material/MenuItem";
import Box from '@mui/material/Box';

const Dropdown = ({ label, items }) => {
  const menuItems = [... new Set(items)].sort();
  return (
    <>
      <Box sx={{ minWidth: 300 }}>
        <InputLabel id="select-label">{label}</InputLabel>
        <Select
          labelId="select-label"
          id="select"
          value="All"
          label={label}
          // onChange={handleChange}
        >
            <MenuItem value="All">All</MenuItem>
          {menuItems.map((item, i) => (
            <MenuItem key={i} value={item}>
              {item}
            </MenuItem>
          ))}
        </Select>
      </Box>
    </>
  );
};

export default Dropdown;

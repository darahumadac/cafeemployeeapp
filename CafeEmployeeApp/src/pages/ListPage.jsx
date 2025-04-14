import React, { useEffect, useState, useRef } from "react";
import { Button, Container } from "@mui/material";
import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TablePagination,
  Paper,
  Box,
  CircularProgress
} from "@mui/material";
// import Table from "../Table.jsx";
import axios from "axios";
import { Link, useLocation } from "react-router-dom";
import { API_URL, DATA_HEADERS } from "../../config.js";
import { AgGridReact } from "ag-grid-react";
import "ag-grid-community/styles/ag-grid.css";
import "ag-grid-community/styles/ag-theme-alpine.css";
import ConfirmModal from "../ConfirmModal.jsx";
import SearchBar from "../SearchBar.jsx";
import Dropdown from "../Dropdown.jsx";

const ListPage = () => {
  const { pathname } = useLocation();
  const entityName = pathname.slice(1, -1);
  const entityPath = pathname.slice(0, -1);

  const GET_LIST_URL = `${API_URL}${pathname}`;
  const DELETE_URL = `${API_URL}${entityPath}`;
  const dropdownRef = useRef(null);

  const [rows, setRows] = useState([]);
  const [loading, setLoading] = useState(true);
  const [reload, setReload] = useState(true);
  const [lastModified, setLastModified] = useState("");

  const [confirmDeleteOpen, setConfirmDeleteOpen] = useState(false);

  const [page, setPage] = useState(0); // Current page
  const [rowsPerPage, setRowsPerPage] = useState(5); // Rows per page

  //dropdown handler
  const handleLocationChange = (event, setSelected) => {
    const selectedItem = event.target.value;
    setSelected(selectedItem);
    if(selectedItem === "All")
    {
      setReload(true);
      return;
    }
    //query records by location
    axios.get(`${GET_LIST_URL}?location=${selectedItem}`).then((response) => setRows(response.data))
  };

  // Pagination handlers
  const handleChangePage = (event, newPage) => setPage(newPage);
  const handleChangeRowsPerPage = (event) => {
    setRowsPerPage(parseInt(event.target.value, 10));
    setPage(0); // Reset to first page
  };

  useEffect(() => {
    loadData();
    console.log(dropdownRef.current)
    if(!dropdownRef.current)
    {
      dropdownRef.current = rows.map((item) => item.location);
      console.log(dropdownRef.current);
    }
  }, [reload]);



  const loadData = () => {
    if(!reload) return;
    console.log("load data:", GET_LIST_URL);
    setLoading(true);
    axios
      .get(GET_LIST_URL, {
        headers: {
          "If-Modified-Since": lastModified,
        },
      })
      .then((response) => {
        const dataList = response.data;
        setLastModified(response.headers["last-modified"]);
        setRows(dataList);
        // setReload(true);
        console.log("done set data row");
      })
      .catch((err) => {
        console.log(err, "error load data");
      })
      .finally(() => {
        setReload(false);
        setLoading(false);
      });
  };

  

  const handleDelete = (e, data) => {
    e.preventDefault();
    setConfirmDeleteOpen(true);
    const toDelete = confirm(`are you sure you want to delete: ${data.name}`);
    if (toDelete) {
      const DELETE_ITEM_URL = `${DELETE_URL}/${data.id}`;
      axios
        .delete(DELETE_ITEM_URL)
        .then((response) => {
          console.log("reloading");
          setLastModified(response.headers["last-modified"]);
          // loadData();
          setReload(true);
        })
        .catch(() => {
          // console.log("error deleting");
        })
        .finally(() => {
          // console.log("done delete refresh");
        });
    }

    //show modal confirm
    //if yes, call
  };



  const [columns] = useState(DATA_HEADERS[pathname].map((h) => ({ field: h })));

  // Get visible rows
  const paginatedRows = rows.slice(
    page * rowsPerPage,
    page * rowsPerPage + rowsPerPage
  );

  return (
    <>
      {/* <ConfirmModal open={confirmDeleteOpen} /> */}
      <Paper>
        <Button onClick={() => setReload(true)}>Refresh</Button>
        {/* {entityName == "employee" && <SearchBar toSearch={entityName} />} */}
        {entityName == "cafe" && (
          <Dropdown
            label={"Location"}
            items={rows.map((item) => item.location)}
            handleChange={handleLocationChange}
          />
        )}
        <Button to={`/${entityName}`} component={Link}>
          Add {entityName}
        </Button>
        <TableContainer>
          <Table>
            <TableHead>
              <TableRow>
                {columns.map((c) => (
                  <TableCell key={c.field}>{c.field.toUpperCase()}</TableCell>
                ))}
                <TableCell></TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {loading &&  (<TableRow>
              <TableCell colSpan={columns.length+2}>
                <Box display="flex" justifyContent="center" alignItems="center" height={100}>
                  <CircularProgress />
                </Box>
              </TableCell>
            </TableRow>) ||
              paginatedRows.map((row) => (
                <TableRow key={row.id}>
                  {columns.map((c) => (
                    <TableCell key={c.field}>
                      {(c.field == "employees" && (
                        <Link
                          to={{
                            pathname: "/employees",
                            search: `?cafe=${row.id}`,
                          }}
                        >
                          {row[c.field]}
                        </Link>
                      )) ||
                        row[c.field]}
                    </TableCell>
                  ))}
                  <TableCell>
                    <Button href={`${pathname}/${row.id}`}>Edit</Button>
                    <Button onClick={(e) => handleDelete(e, row)}>
                      Delete
                    </Button>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>

        {/* foorter */}
        <TablePagination
          component="div"
          count={rows.length}
          page={page}
          onPageChange={handleChangePage}
          rowsPerPage={rowsPerPage}
          onRowsPerPageChange={handleChangeRowsPerPage}
          rowsPerPageOptions={[5, 10, 25]}
        />
      </Paper>
    </>
  );
};

export default ListPage;
